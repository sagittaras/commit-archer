using Sagittaras.CommitArcher.Changelog.Source;
using Sagittaras.CommitArcher.Core;
using SlackNet.Blocks;

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
    public async Task<(string, IList<Block>)> GenerateMessageAsync()
    {
        IChangelogResult result = await source.GetChangelogAsync();
        string fallback = $"A new version {result.Version} has been released.";

        List<Block> blocks =
        [
            new HeaderBlock
            {
                Text = new PlainText
                {
                    Emoji = true,
                    Text = ":rocket: New version has been released!"
                }
            }
        ];

        if (!string.IsNullOrEmpty(result.VersionDescription))
        {
            blocks.Add(new ContextBlock
            {
                Elements =
                [
                    new Markdown(result.VersionDescription)
                ]
            });
        }

        blocks.Add(new SectionBlock
        {
            Text = new Markdown($"Changelist for version *{result.Version}*")
        });

        foreach ((string type, string heading) in CommitTypes)
        {
            blocks.Add(new DividerBlock());
            blocks.Add(new SectionBlock
            {
                Text = new Markdown(heading)
            });

            foreach (IConventionalCommit commit in result.Commits.Where(x => x.Type == type))
            {
                blocks.Add(new SectionBlock
                {
                    Text = new PlainText(commit.Description)
                });

                if (!string.IsNullOrEmpty(commit.Body))
                {
                    blocks.Add(new ContextBlock
                    {
                        Elements =
                        [
                            new PlainText(commit.Body)
                        ]
                    });
                }
            }
        }

        return (fallback, blocks);
    }
}