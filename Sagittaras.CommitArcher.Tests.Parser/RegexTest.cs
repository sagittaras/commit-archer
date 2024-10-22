using System.Text.RegularExpressions;
using FluentAssertions;
using Sagittaras.CommitArcher.Parser;

namespace Sagittaras.CommitArcher.Tests.Parser;

/// <summary>
///     Tests the regular expression to parse the conventional commit format.
/// </summary>
public class RegexTest
{
    /// <summary>
    ///     Tests a simple commit message with a type and description.
    /// </summary>
    [Fact]
    public void Test_SimpleMessage()
    {
        const string message = "feat: This is a simple message.";

        Match match = ConventionalCommitParser.ConventionalCommitRegex().Match(message);
        match.Success.Should().BeTrue();
        
        match.Groups
            .OfType<Group>()
            .Where(x => !string.IsNullOrEmpty(x.Value))
            .Should()
            .HaveCount(3); // 2 Groups + Match.

        match.Groups.TryGetValue("type", out Group? type).Should().BeTrue();
        type!.Value.Should().Be("feat");
        
        match.Groups.TryGetValue("description", out Group? description).Should().BeTrue();
        description!.Value.Should().Be("This is a simple message.");
    }

    /// <summary>
    ///     Simple message which contains the scope.
    /// </summary>
    [Fact]
    public void Test_WithScope()
    {
        const string message = "fix(core): This is a message with scope.";
        
        Match match = ConventionalCommitParser.ConventionalCommitRegex().Match(message);
        match.Success.Should().BeTrue();
        
        match.Groups.TryGetValue("scope", out Group? scope).Should().BeTrue();
        scope!.Value.Should().Be("core");
    }

    /// <summary>
    ///     Message which contains the body.
    /// </summary>
    [Fact]
    public void Test_WithBody()
    {
        const string message = """
                               fix(core): This is a message with body.
                               
                               This is our body.
                               """;
        
        Match match = ConventionalCommitParser.ConventionalCommitRegex().Match(message);
        match.Success.Should().BeTrue();
        
        match.Groups.TryGetValue("body", out Group? body).Should().BeTrue();
        body!.Value.Should().Be("This is our body.");
    }

    /// <summary>
    ///     Message containing a full specification abilities.
    /// </summary>
    [Fact]
    public void Test_FullSpecification()
    {
        const string message = """
                               fix(core): This is a full spec message.
                               
                               Contains a body.
                               
                               BREAKING CHANGE: Description for breaking change.
                               Reviewed-by: Developer
                               """;
        
        Match match = ConventionalCommitParser.ConventionalCommitRegex().Match(message);
        match.Success.Should().BeTrue();

        match.Groups.TryGetValue("footers", out Group? footers).Should().BeTrue();
        match.Groups.TryGetValue("breaking_change", out Group? breakingChange).Should().BeTrue();
        match.Groups.TryGetValue("breaking_description", out Group? breakingDescription).Should().BeTrue();

        footers!.Value.Should().Be("""
                                  BREAKING CHANGE: Description for breaking change.
                                  Reviewed-by: Developer
                                  """);
        breakingChange!.Value.Should().Be("BREAKING CHANGE: Description for breaking change.");
        breakingDescription!.Value.Should().Be("Description for breaking change.");
    }

    /// <summary>
    ///     Message is containing a breaking change by usage of exclamation mark.
    /// </summary>
    [Fact]
    public void Test_BreakingByExclamation()
    {
        const string message = "feat(core)!: Breaking change commit.";
        
        Match match = ConventionalCommitParser.ConventionalCommitRegex().Match(message);
        match.Success.Should().BeTrue();
        
        match.Groups.TryGetValue("breaking", out Group? _).Should().BeTrue();
    }
}