namespace Sagittaras.CommitArcher.Changelog.Source.GitHub;

/// <summary>
///     A builder class for constructing instances of <see cref="GitHubChangelogSource"/>.
/// </summary>
public class GitHubChangelogSourceBuilder
{
    /// <summary>
    ///     The personal access token used for authenticating with the GitHub API.
    /// </summary>
    internal string Token { get; private set; } = string.Empty;

    /// <summary>
    ///     The owner of the repository from which the commits will be fetched.
    /// </summary>
    internal string RepositoryOwner { get; private set; } = string.Empty;

    /// <summary>
    ///     The name of the GitHub repository from which commits will be fetched.
    /// </summary>
    internal string RepositoryName { get; private set; } = string.Empty;

    /// <summary>
    ///     The name of the branch in the GitHub repository from which commits will be fetched.
    /// </summary>
    internal string BranchName { get; private set; } = "main";

    /// <summary>
    ///     Builds and returns an instance of <see cref="GitHubChangelogSource"/>.
    /// </summary>
    /// <returns>An instance of <see cref="GitHubChangelogSource"/>.</returns>
    public GitHubChangelogSource Build()
    {
        return new GitHubChangelogSource(this);
    }

    /// <summary>
    ///     Sets the token to be used for accessing the GitHub API.
    /// </summary>
    /// <param name="token">The token for authenticating with the GitHub API.</param>
    /// <returns>The <see cref="GitHubChangelogSourceBuilder"/> instance with the specified token.</returns>
    public GitHubChangelogSourceBuilder UseToken(string token)
    {
        Token = token;
        return this;
    }

    /// <summary>
    ///     Sets the repository details to be used for accessing the GitHub API.
    /// </summary>
    /// <param name="repositoryOwner">The owner of the GitHub repository.</param>
    /// <param name="repositoryName">The name of the GitHub repository.</param>
    /// <returns>The <see cref="GitHubChangelogSourceBuilder"/> instance with the specified repository details.</returns>
    public GitHubChangelogSourceBuilder UseRepository(string repositoryOwner, string repositoryName)
    {
        RepositoryOwner = repositoryOwner;
        RepositoryName = repositoryName;
        return this;
    }

    /// <summary>
    ///     Sets the branch to be used for accessing the GitHub API.
    /// </summary>
    /// <param name="branchName">The name of the branch to be used.</param>
    /// <returns>The <see cref="GitHubChangelogSourceBuilder"/> instance with the specified branch.</returns>
    public GitHubChangelogSourceBuilder UseBranch(string branchName)
    {
        BranchName = branchName;
        return this;
    }
}