using FluentAssertions;
using Moq;
using Sagittaras.CommitArcher.Changelog;
using Sagittaras.CommitArcher.Changelog.Markdown;
using Sagittaras.CommitArcher.Changelog.Source;
using Sagittaras.CommitArcher.Core;

namespace Sagittaras.CommitArcher.Tests.Changelog.Markdown;

public class MarkdownChangelogGeneratorTest
{
    /// <summary>
    ///     Mocked result of the data from changelog.
    /// </summary>
    /// <remarks>
    ///     For some real representation of the changelog we decided to take a changelog from our latest version of Spellborn for GameDev Connect.
    /// </remarks>
    private readonly ChangelogResult _result = new()
    {
        Version = "0.3.8-alpha",
        Commits =
        [
            new ConventionalCommit
            {
                Type = "chore",
                Description = "bump version to 0.3.8-alpha in ProjectSettings.asset"
            },
            new ConventionalCommit
            {
                Type = "feat",
                Description = "Waypoint is also animated."
            },
            new ConventionalCommit
            {
                Type = "feat",
                Description = "Added animation to the quest markers.",
                Body = "To make things more living on the map."
            },
            new ConventionalCommit
            {
                Type = "feat",
                Description = "Sound effect playing when the ambush battle is skipped yet it provided a loot from enemy."
            },
            new ConventionalCommit
            {
                Type = "fix",
                Description = "After player has finished its movement it is not being ported back and forth."
            },
            new ConventionalCommit
            {
                Type = "fix",
                Description = "When Hud Presenter is dismissed, the controls of player are blocked.",
                Body = "This helps to prevent unwanted clicks through the other presenters to the interactive map."
            }
        ]
    };

    /// <summary>
    ///     The expected result string representing the changelog in Markdown format for testing purposes.
    /// </summary>
    /// <remarks>
    ///     Replacement of Line Endings helps to synchronize the expected string with the result which is appending by usage of environment's
    ///     new line.
    /// </remarks>
    private readonly string _expectedResult = """
                                              # 🚀 Version 0.3.8-alpha

                                              ## ✨ Features
                                              - **Waypoint is also animated.**
                                              - **Added animation to the quest markers.**
                                              	- _To make things more living on the map._
                                              - **Sound effect playing when the ambush battle is skipped yet it provided a loot from enemy.**
                                              
                                              ## 🐛 Fixes
                                              - **After player has finished its movement it is not being ported back and forth.**
                                              - **When Hud Presenter is dismissed, the controls of player are blocked.**
                                              	- _This helps to prevent unwanted clicks through the other presenters to the interactive map._
                                              
                                              """.ReplaceLineEndings(Environment.NewLine);

    /// <summary>
    ///     Provides a mocked source for retrieving changelog data.
    /// </summary>
    private IChangelogSource ChangelogSource
    {
        get
        {
            Mock<IChangelogSource> mock = new();
            mock.Setup(x => x.GetLatestChangelogAsync()).ReturnsAsync(_result);

            return mock.Object;
        }
    }

    /// <summary>
    ///     Tests generating of the changelog in Markdown format.
    /// </summary>
    [Fact]
    public async Task Test_Generate()
    {
        MarkdownChangelogGenerator generator = new(ChangelogSource);

        string changelog = await generator.GenerateAsync();
        changelog.Should().Be(_expectedResult);
    }
}