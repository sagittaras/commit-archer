using FluentAssertions;
using Sagittaras.CommitArcher.Core;
using Sagittaras.CommitArcher.Parser;

namespace Sagittaras.CommitArcher.Tests.Parser;

/// <summary>
///     Tests ability of <see cref="ConventionalCommitParser"/> of parsing the commit message to object.
/// </summary>
public class ConventionalCommitParserTest
{
    /// <summary>
    ///     Parses a simple message with the required values only.
    /// </summary>
    [Fact]
    public void Test_SimpleMessage()
    {
        IConventionalCommit commit = ConventionalCommitParser.ParseCommit("Feat: This is a simple message.");
        commit.Type.Should().Be("feat");
        commit.Description.Should().Be("This is a simple message.");
    }

    /// <summary>
    ///     Parses a simple message with a scope.
    /// </summary>
    [Fact]
    public void Test_WithScope()
    {
        IConventionalCommit commit = ConventionalCommitParser.ParseCommit("fix(Core): This is a message with scope.");
        commit.Scope.Should().Be("core");
    }

    /// <summary>
    ///     Parses a commit message which is containing the body.
    /// </summary>
    [Fact]
    public void Test_WithBody()
    {
        IConventionalCommit commit = ConventionalCommitParser.ParseCommit("""
                                                        fix(core): This is a message with body.

                                                        This is our body.
                                                        """);

        commit.Body.Should().Be("This is our body.");
    }

    /// <summary>
    ///     Parses a commit message containing all specification abilities.
    /// </summary>
    [Fact]
    public void Test_FullSpecification()
    {
        IConventionalCommit commit = ConventionalCommitParser.ParseCommit("""
                                                        fix(core): This is a full spec message.

                                                        Contains a body.

                                                        BREAKING CHANGE: Description for breaking change.
                                                        Reviewed-by: Developer
                                                        """);

        commit.IsBreakingChange.Should().BeTrue();
        commit.BreakingDescription.Should().Be("Description for breaking change.");
        commit.Footers.Should().ContainKey("Reviewed-by");
        commit.Footers["Reviewed-by"].Should().Be("Developer");
    }

    /// <summary>
    ///     Parses a commit message which breaking change is marked by exclamation mark.
    /// </summary>
    [Fact]
    public void Test_BreakingByExclamation()
    {
        IConventionalCommit commit = ConventionalCommitParser.ParseCommit("feat(core)!: Breaking change commit.");
        commit.IsBreakingChange.Should().BeTrue();
        commit.BreakingDescription.Should().BeNull();
    }

    /// <summary>
    ///     Tries to parse a commit message which is not in the expected format.
    /// </summary>
    [Fact]
    public void Test_NonConventionalCommit()
    {
        Action act = () => ConventionalCommitParser.ParseCommit("This is a standard commit message not following the format.");
        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    ///     GitHub is making unexpected escape sequence after body and before footer. This is causing to regular expression to be stucked.
    /// </summary>
    [Fact]
    public void Test_GitHubFooterResolution()
    {
        const string message = "feat: Description\n\nOptional body.\r\n\r\nIssue: CA-000";
        
        IConventionalCommit commit = ConventionalCommitParser.ParseCommit(message);
        commit.Should().NotBeNull();
        commit.Description.Should().Be("Description");
        commit.Footers.Should().HaveCount(1);
        commit.Footers.Should().ContainKey("Issue");
        commit.Footers["Issue"].Should().Be("CA-000");
    }
}