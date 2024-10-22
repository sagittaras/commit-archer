﻿using Octokit;
using Sagittaras.CommitArcher.Changelog;
using Sagittaras.CommitArcher.Core;
using Sagittaras.CommitArcher.Parser;

namespace Sagittaras.CommitArcher.Source.GitHub;

/// <summary>
///     Source implementation for GitHub reading the commits via its API and constructing the changelog.
/// </summary>
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

    internal GitHubChangelogSource(GitHubChangelogSourceBuilder builder)
    {
        _builder = builder;
        _client.Credentials = new Credentials(builder.Token);
    }

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
    public async Task<IChangelogResult> GetLatestChangelogAsync()
    {
        await FindTheVersionCommitAsync();

        List<IConventionalCommit> releaseCommits = [];
        while (true)
        {
            if (!CommitsQueue.TryDequeue(out IConventionalCommit? commit))
            {
                await GetNextCommitsAsync();
                continue;
            }

            if (commit.Type == "release" && commit.Scope == ReleaseScope)
            {
                break;
            }
            
            releaseCommits.Add(commit);
        }

        _result.Commits = releaseCommits.AsReadOnly();
        
        return _result;
    }

    /// <summary>
    ///     Find the first commit marked as "release".
    /// </summary>
    private async Task FindTheVersionCommitAsync()
    {
        string defaultValue = _result.Version;
        
        while (_result.Version == defaultValue)
        {
            if (!CommitsQueue.TryDequeue(out IConventionalCommit? commit))
            {
                await GetNextCommitsAsync();
                continue;
            }

            if (commit.Type != "release")
            {
                continue;
            }

            _result.Version = commit.Description;
            _result.VersionDescription = commit.Body ?? string.Empty;
            ReleaseScope = commit.Scope ?? throw new InvalidOperationException("Release's scope has been expected to be set.");
        }
    }

    /// <summary>
    ///     Retrieves the next collection of conventional commits from the GitHub repository asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of conventional commits.</returns>
    private async Task GetNextCommitsAsync()
    {
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
            throw new InvalidOperationException("No more commits found.");
        }

        List<IConventionalCommit> conventionals = [];
        foreach (GitHubCommit gitHubCommit in commits)
        {
            try
            {
                conventionals.Add(ConventionalCommitParser.ParseCommit(gitHubCommit.Commit.Message));
            }
            catch (ArgumentException)
            {
                // Just catch. Argument exception is thrown when the commit is not in conventional format.
            }
        }

        CommitsQueue = new Queue<IConventionalCommit>(conventionals);
    }
}