using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Sagittaras.CommitArcher.Changelog;
using Sagittaras.CommitArcher.Changelog.Source;
using Sagittaras.CommitArcher.Changelog.Source.GitHub;

namespace Sagittaras.CommitArcher.Tests.Source.GitHub;

public class GitHubChangelogSourceTest
{
    /// <summary>
    ///     Configuration reading the basic initials from the user-secrets.
    /// </summary>
    private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .AddUserSecrets<GitHubChangelogSourceTest>()
        .Build();

    /// <summary>
    ///     Test retrieving of the latest changelog from the repository.
    /// </summary>
    [Fact]
    public async Task Test_LatestChangelog()
    {
        GitHubChangelogSource source = new GitHubChangelogSourceBuilder()
            .UseToken(_configuration["Token"]!)
            .UseRepository(_configuration["RepositoryOwner"]!, _configuration["RepositoryName"]!)
            .UseBranch(_configuration["BranchName"]!)
            .Build();

        IChangelogResult result = await source.GetLatestChangelogAsync();
        result.Version.Should().NotBeEmpty();
        result.Commits.Should().NotBeEmpty();
    }
}