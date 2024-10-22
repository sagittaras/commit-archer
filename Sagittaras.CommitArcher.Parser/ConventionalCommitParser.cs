using System.Text.RegularExpressions;
using Sagittaras.CommitArcher.Core;
using Sagittaras.CommitArcher.Parser.Extensions;

namespace Sagittaras.CommitArcher.Parser;

/// <summary>
///     Provides functionality to parse commit messages adhering to the conventional commit specification.
/// </summary>
/// <see href="https://www.conventionalcommits.org/en/v1.0.0/"/>
/// <seealso cref="IConventionalCommit"/>
public static partial class ConventionalCommitParser
{
    /// <summary>
    ///     Constructs an IConventionalCommit object from a commit message string.
    /// </summary>
    /// <param name="message">The string containing the commit message to parse.</param>
    /// <returns>An IConventionalCommit object populated with details extracted from the commit message.</returns>
    public static IConventionalCommit ParseCommit(string message)
    {
        Match match = ConventionalCommitRegex().Match(message);
        if (!match.Success)
        {
            throw new ArgumentException("Invalid commit message format. Unable to parse.");
        }

        return new ConventionalCommit
        {
            Type = match.GetCommitType(),
            Scope = match.GetCommitScope(),
            Description = match.GetCommitDescription(),
            Body = match.GetCommitBody(),
            IsBreakingChange = match.IsCommitBreakingChange(),
            BreakingDescription = match.GetBreakingDescription(),
            Footers = ParseCommitFooters(match)
        };
    }

    /// <summary>
    ///     Parses the commit footers from a match object and returns them as a dictionary of key-value pairs.
    /// </summary>
    /// <param name="match">The Match object containing the commit details.</param>
    /// <returns>A dictionary where keys are footer names and values are footer contents.</returns>
    private static Dictionary<string, string> ParseCommitFooters(Match match)
    {
        string footersString = match.GetCommitFooters();
        if (string.IsNullOrEmpty(footersString))
        {
            return [];
        }

        const StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
        return footersString.Split("\n", splitOptions)
                .Select(line => line.Split(":", splitOptions))
                .ToDictionary(x => x[0], x => x[1])
            ;
    }

    /// <summary>
    ///     Gets a regular expression that matches commit messages following the conventional commit specification.
    /// </summary>
    /// <remarks>
    ///     The commit message is parsed into these groups:
    ///     - <c>type</c>
    ///     - <c>scope</c>
    ///     - <c>breaking</c>, identifies whether the exclamation mark was used to mark braking change.
    ///     - <c>description</c>
    ///     - <c>body</c>
    ///     - <c>breaking_change</c>, identifies whether <c>"BREAKING CHANGE"</c> footer with description has been used.
    ///     - <c>breaking_description</c>, description from the breaking change footer.
    ///     - <c>footers</c>, optional footers contained in the commit message.
    /// </remarks>
    /// <returns>A <see cref="Regex"/> object that can be used to identify and parse conventional commit messages.</returns>
    [GeneratedRegex(
        @"\A(?:(?:^(?<type>\w+)(?:\((?<scope>[\w-]+)\))?(?<breaking>!)?:\s(?<description>\b[\w#<> ./\t\\-]{3,}(?:\b|\.))$)(?:\n^$\n(?<body>(?:(?!^BREAKING\sCHANGE:|^\w+(?:-\w+)*:).*\n?)*))?(?:\n^$\n(?<footers>(?:(?<breaking_change>^BREAKING\sCHANGE:\s(?<breaking_description>.+(?:\b|\.)$))\n)?(?:^\w+(?:-\w+)*:\s[^\n]+\n*)*))?\n?|)\Z",
        RegexOptions.Multiline
    )]
    public static partial Regex ConventionalCommitRegex();
}