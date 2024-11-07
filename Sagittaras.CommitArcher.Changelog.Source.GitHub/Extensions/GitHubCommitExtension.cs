using Octokit;
using Sagittaras.CommitArcher.Core;
using Commit = Sagittaras.CommitArcher.Core.Commit;

namespace Sagittaras.CommitArcher.Changelog.Source.GitHub.Extensions;

public static class GitHubCommitExtension
{
    /// <summary>
    ///     Converts the GitHubCommit response object to an object described by the
    ///     CommitArcher library.
    /// </summary>
    /// <param name="commit"></param>
    /// <returns></returns>
    public static ICommit ToGitCommit(this GitHubCommit commit)
    {
        return new Commit
        {
            Author = commit.Commit.Author.Name,
            AuthorEmail = commit.Commit.Author.Email,
            Authored = commit.Commit.Author.Date.UtcDateTime,
            
            Committer = commit.Commit.Committer.Name,
            CommitterEmail = commit.Commit.Committer.Email,
            Committed = commit.Commit.Committer.Date.UtcDateTime,
            
            Sha = commit.Sha,
            Url = commit.Url
        };
    }
}