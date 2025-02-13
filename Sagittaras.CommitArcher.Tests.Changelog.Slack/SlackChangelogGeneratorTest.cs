using Meziantou.Extensions.Logging.Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sagittaras.CommitArcher.Changelog.Slack;
using Sagittaras.CommitArcher.Tests.Changelog.Slack.Logging;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.WebApi;
using Xunit.Abstractions;
using ILogger = SlackNet.ILogger;

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
    ///     Provides access to the service provider, which offers dependency injection and resolves service dependencies for the application.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;
    
    public SlackChangelogGeneratorTest(ITestOutputHelper testOutputHelper)
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
        services.AddSingleton<ILogger, SlackNetLogger>();
        
        _serviceProvider = services.BuildServiceProvider();
    }
    
    /// <summary>
    ///     Tests sending a message to a Slack channel.
    /// </summary>
    [Fact]
    public async Task Test_SendSlackMessage()
    {
        SlackChangelogGenerator generator = new(ChangelogSourceMock.Create());
        (string plainText, IList<Block> blocks) = await generator.GenerateMessageAsync();

        ISlackApiClient client = new SlackServiceBuilder()
            .UseApiToken(_configuration["SlackToken"])
            .UseLogger(_ => _serviceProvider.GetRequiredService<ILogger>())
            .GetApiClient();

        await client.Chat.PostMessage(new Message
        {
            Text = plainText,
            Blocks = blocks,
            Channel = _configuration["ChannelId"]
        });
    }
}