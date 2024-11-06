using Sagittaras.CommitArcher.Changelog.Source;
using Sagittaras.CommitArcher.Core;
using SlackAPI;

namespace Sagittaras.CommitArcher.Changelog.Slack;

/// <summary>
///     Generates the changelog in as block message suitable to be sent to a Slack channel.
/// </summary>
public class SlackChangelogGenerator(IChangelogSource source) : ChangelogGenerator
{
    /// <summary>
    ///     Generates the changelog as a block message suitable to be sent to a Slack channel.
    /// </summary>
    /// <returns>Tuple containing a plain text message as a fallback for notifications and an array of blocks representing the message in a rich format.</returns>
    public async Task<(string, ICollection<IBlock>)> GenerateMessageAsync()
    {
        IChangelogResult result = await source.GetLatestChangelogAsync();
        string fallback = $"A new version {result.Version} has been released.";

        List<IBlock> blocks =
        [
            new HeaderBlock
            {
                text = new Text
                {
                    type = "plain_text",
                    emoji = true,
                    text = ":rocket: New version has been released!"
                }
            }
        ];

        if (!string.IsNullOrEmpty(result.VersionDescription))
        {
            blocks.Add(new ContextBlock
            {
                elements =
                [
                    new Text
                    {
                        type = "mrkdwn",
                        text = result.VersionDescription
                    }
                ]
            });
        }

        blocks.Add(new SectionBlock
        {
            text = new Text
            {
                type = "mrkdwn",
                text = $"Changelist for version *{result.Version}*"
            }
        });

        foreach ((string type, string heading) in CommitTypes)
        {
            blocks.Add(new DividerBlock());
            blocks.Add(new SectionBlock
            {
                text = new Text
                {
                    type = "mrkdwn",
                    text = heading
                }
            });

            foreach (IConventionalCommit commit in result.Commits.Where(x => x.Type == type))
            {
                blocks.Add(new SectionBlock
                {
                    text = new Text
                    {
                        type = "plain_text",
                        text = commit.Description
                    }
                });

                if (!string.IsNullOrEmpty(commit.Body))
                {
                    blocks.Add(new ContextBlock
                    {
                        elements =
                        [
                            new Text
                            {
                                type = "plain_text",
                                text = commit.Body
                            }
                        ]
                    });
                }
            }
        }

        return (fallback, blocks);
    }
}