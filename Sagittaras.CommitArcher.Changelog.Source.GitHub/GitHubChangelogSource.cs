using Microsoft.Extensions.Logging;
using Octokit;
using Sagittaras.CommitArcher.Changelog.Source.GitHub.Extensions;
using Sagittaras.CommitArcher.Core;
using Sagittaras.CommitArcher.Parser;

namespace Sagittaras.CommitArcher.Changelog.Source.GitHub;

/// <summary>
///     Source implementation for GitHub reading the commits via its API and constructing the changelog.
/// </summary>
/// <remarks>
///     Introduces approach that is in usage by Sagittaras Games team in their development pipeline. Environmental or release branches
///     are used. Every new release through this branch is marked via merge commit in conventional format of type <c>release</c>, where
///     its scope describes the environment. This allows to promote updates through different environments.
///     <c>release</c> commit should contain the version number in its description, and it can contain the further description of the release in its
///     optional body part.
///     Class traverses the commit history from the resolved version until it encounters a <c>release</c> commit in the same scope or until the end of the history is reached.
/// </remarks>
public class GitHubChangelogSource : IChangelogSource
{
    /// <summary>
    ///     Instance of client accessing the GitHub API.
    /// </summary>
    private readonly GitHubClient _client = new(new ProductHeaderValue("CommitArcher"));

    /// <summary>
    ///     Builder instance used to configure and create <see cref="GitHubChangelogSource"/> objects.
    /// </summary>
    private readonly GitHubChangelogSourceBuilder _builder;

    /// <summary>
    ///     Stores the result of the changelog generation process.
    /// </summary>
    private readonly ChangelogResult _result = new();

    /// <summary>
    ///     Stores the resolved version of the repository after fetching from the GitHub API.
    /// </summary>
    private string _resolvedVersion = string.Empty;

    /// <summary>
    ///     Holds information about the release commit.
    /// </summary>
    private IConventionalCommit? _releaseCommit;

    internal GitHubChangelogSource(GitHubChangelogSourceBuilder builder)
    {
        _builder = builder;
        _client.Credentials = new Credentials(builder.Token);

        Logger.LogInformation(
            "GitHub Changelog source for {Repository}/{Owner} [{Branch}] has been created.",
            _builder.RepositoryOwner,
            _builder.RepositoryName,
            _builder.BranchName
        );
    }

    /// <summary>
    ///     Shortcut access to the logger.
    /// </summary>
    private ILogger Logger => _builder.Logger;

    /// <summary>
    ///     Represents the current page number used for paginated requests to the GitHub API.
    /// </summary>
    private int Page { get; set; } = 1;

    /// <summary>
    ///     A queue of loaded commits to be processed into changelog.
    /// </summary>
    private Queue<IConventionalCommit> CommitsQueue { get; set; } = new();

    /// <summary>
    ///     The scope of the version release we are searching for.
    /// </summary>
    /// <remarks>
    ///     The scope helps to find the previous release of the same environment.
    /// </remarks>
    private string ReleaseScope { get; set; } = string.Empty;

    /// <inheritdoc />
    public string ResolvedVersion => !string.IsNullOrEmpty(_resolvedVersion) ? _resolvedVersion : throw new InvalidOperationException("The version in the repository has not been resolved yet.");

    /// <inheritdoc />
    public IConventionalCommit ReleaseCommit => _releaseCommit ?? throw new InvalidOperationException("The release commit has not been resolved yet.");

    /// <inheritdoc />
    public async Task<string> ResolveLatestVersionAsync()
    {
        await FindTheVersionAsync();
        return ResolvedVersion;
    }

    /// <inheritdoc />
    public Task ResolveVersionAsync(string version)
    {
        return FindTheVersionAsync(version);
    }

    /// <inheritdoc />
    public async Task<IChangelogResult> GetChangelogAsync()
    {
        if (string.IsNullOrEmpty(_resolvedVersion))
        {
            Logger.LogInformation("Version has not been resolved yet");
            await FindTheVersionAsync();
        }

        List<IConventionalCommit> releaseCommits = [];
        while (true)
        {
            if (!CommitsQueue.TryDequeue(out IConventionalCommit? commit))
            {
                try
                {
                    await GetNextCommitsAsync();
                }
                catch (InvalidOperationException)
                {
                    Logger.LogInformation("The changelog has reached the end of history");
                    break;
                }

                continue;
            }

            if (commit.Type == "release" && commit.Scope == ReleaseScope)
            {
                Logger.LogInformation("Reached end of changelog for version {Version}, found {Commits} commits in total", _result.Version, releaseCommits.Count);
                break;
            }

            Logger.LogTrace("{Commit}", commit.ToString());
            releaseCommits.Add(commit);
        }

        releaseCommits.Reverse(); // We are reading from the latest commit. Once the changelog is ready, reverse the order.
        _result.Commits = releaseCommits.AsReadOnly();
        _result.ReleaseCommit = ReleaseCommit;

        return _result;
    }

    /// <summary>
    ///     Find the first commit marked as <c>release</c>.
    /// </summary>
    /// <param name="version">Specific version to be found in the commits. If not set, the latest version is resolved.</param>
    private async Task FindTheVersionAsync(string? version = null)
    {
        if (version is null)
        {
            Logger.LogInformation("Searching for latest version in commits history");
        }
        else
        {
            Logger.LogInformation("Searching for version {Version} in commits history", version);
        }

        while (string.IsNullOrEmpty(_resolvedVersion))
        {
            if (!CommitsQueue.TryDequeue(out IConventionalCommit? commit))
            {
                try
                {
                    await GetNextCommitsAsync();
                }
                catch (InvalidOperationException e)
                {
                    Logger.LogError("The changelog has reached the end of history without specific release version");
                    throw new InvalidOperationException("The changelog has reached the end of history without specific release version.", e);
                }

                continue;
            }

            if (commit.Type != "release")
            {
                continue;
            }

            // We are searching for specific version.
            if (version is not null && commit.Description != version)
            {
                Logger.LogDebug("Found version {FoundVersion}, but searching for specific version {Version}", commit.Description, version);
                continue;
            }

            _result.Version = _resolvedVersion = commit.Description;
            _result.VersionDescription = commit.Body ?? string.Empty;
            _releaseCommit = commit;
            ReleaseScope = commit.Scope ?? throw new InvalidOperationException("Release's scope has been expected to be set.");

            Logger.LogInformation("Resolved version {Version} in scope {Scope} on commit {Sha}", _result.Version, ReleaseScope, commit.OriginalCommit.Sha);
            Logger.LogInformation(
                "Version {Version} has been committed by {Committer} and authored by {Authored}",
                _result.Version,
                commit.OriginalCommit.Committer,
                commit.OriginalCommit.Author
            );
        }
    }

    /// <summary>
    ///     Retrieves the next collection of conventional commits from the GitHub repository asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of conventional commits.</returns>
    private async Task GetNextCommitsAsync()
    {
        Logger.LogDebug("Loading page {Page} of commits history with size 30", Page);

        IReadOnlyList<GitHubCommit> commits = await _client.Repository.Commit.GetAll(_builder.RepositoryOwner, _builder.RepositoryName, new CommitRequest
        {
            Sha = _builder.BranchName
        }, new ApiOptions
        {
            StartPage = Page++,
            PageSize = 30,
            PageCount = 1
        });

        if (commits.Count == 0)
        {
            Logger.LogWarning(
                "No more commits has been found in {Owner}/{Repository} for Branch {Branch}",
                _builder.RepositoryOwner,
                _builder.RepositoryName,
                _builder.BranchName
            );
            throw new InvalidOperationException("No more commits found.");
        }

        List<IConventionalCommit> resolved = [];
        foreach (GitHubCommit gitHubCommit in commits)
        {
            try
            {
                IConventionalCommit commit = ConventionalCommitParser.ParseCommit(gitHubCommit.Commit.Message);
                commit.OriginalCommit = gitHubCommit.ToGitCommit();

                resolved.Add(commit);
            }
            catch (ArgumentException e)
            {
                Logger.LogWarning(e, "Commit {Sha} is not in correct conventional commit format", gitHubCommit.Sha);
            }
        }

        CommitsQueue = new Queue<IConventionalCommit>(resolved);
    }
}