using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Sagittaras.CommitArcher.Changelog.Slack;
using SlackAPI;

namespace Sagittaras.CommitArcher.Tests.Changelog.Slack;

public class SlackChangelogGeneratorTest
{
    /// <summary>
    ///     Configuration reading the basic initials from the user secrets.
    /// </summary>
    private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .AddUserSecrets<SlackChangelogGeneratorTest>()
        .Build();
    
    /// <summary>
    ///     Tests sending a message to a Slack channel.
    /// </summary>
    [Fact]
    public async Task Test_SendSlackMessage()
    {
        SlackChangelogGenerator generator = new(ChangelogSourceMock.Create());
        (string plainText, ICollection<IBlock> blocks) = await generator.GenerateMessageAsync();

        SlackTaskClient client = new(_configuration["SlackToken"]);
        PostMessageResponse response = await client.PostMessageAsync(_configuration["ChannelId"], plainText, blocks: blocks.ToArray(), unfurl_links: false);
        response.ok.Should().BeTrue();
    }
}