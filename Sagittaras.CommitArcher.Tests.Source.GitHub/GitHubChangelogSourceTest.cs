using FluentAssertions;
using Meziantou.Extensions.Logging.Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sagittaras.CommitArcher.Changelog;
using Sagittaras.CommitArcher.Changelog.Source;
using Sagittaras.CommitArcher.Changelog.Source.GitHub;
using Xunit.Abstractions;

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
    ///     Service Provider for creating a Logger build from test output helper.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    public GitHubChangelogSourceTest(ITestOutputHelper testOutputHelper)
    {
        ServiceCollection services = [];
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddProvider(new XUnitLoggerProvider(testOutputHelper, new XUnitLoggerOptions
            {
                IncludeLogLevel = true,
                IncludeCategory = true
            }));
        });

        _serviceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    ///     Test retrieving of the latest changelog from the repository.
    /// </summary>
    /// <remarks>
    ///     The version has not been resolved before the <see cref="GitHubChangelogSource.GetChangelogAsync"/> call. It should be resolved automatically.
    /// </remarks>
    [Fact]
    public async Task Test_LatestChangelog()
    {
        GitHubChangelogSource source = new GitHubChangelogSourceBuilder()
            .UseToken(_configuration["Token"]!)
            .UseRepository(_configuration["RepositoryOwner"]!, _configuration["RepositoryName"]!)
            .UseBranch(_configuration["BranchName"]!)
            .UseLogger(_serviceProvider.GetRequiredService<ILogger<GitHubChangelogSource>>())
            .Build();

        IChangelogResult result = await source.GetChangelogAsync();
        result.Version.Should().NotBeEmpty();
        result.Commits.Should().NotBeEmpty();
    }

    /// <summary>
    ///     Test retrieving the changelog by specific version.
    /// </summary>
    [Fact]
    public async Task Test_ChangelogByResolvedVersion()
    {
        GitHubChangelogSource source = new GitHubChangelogSourceBuilder()
            .UseToken(_configuration["Token"]!)
            .UseRepository(_configuration["RepositoryOwner"]!, _configuration["RepositoryName"]!)
            .UseBranch(_configuration["BranchName"]!)
            .UseLogger(_serviceProvider.GetRequiredService<ILogger<GitHubChangelogSource>>())
            .Build();

        await source.ResolveVersionAsync(_configuration["ResolveVersion"]!);
        IChangelogResult result = await source.GetChangelogAsync();
        
        result.Version.Should().NotBeEmpty();
        result.Commits.Should().HaveCount(int.Parse(_configuration["ResolvedVersionCommits"]!));
    }
}