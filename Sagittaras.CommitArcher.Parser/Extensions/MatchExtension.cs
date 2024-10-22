using System.Text.RegularExpressions;

namespace Sagittaras.CommitArcher.Parser.Extensions;

internal static class MatchExtension
{
    /// <summary>
    ///     Retrieves a named group from the match. If the group name does not exist, an exception is thrown.
    /// </summary>
    /// <param name="match">The Match object containing the groups.</param>
    /// <param name="groupName">The name of the group to retrieve.</param>
    /// <returns>The Group object corresponding to the specified group name.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the group name is not found in the match.</exception>
    private static Group GetGroup(this Match match, string groupName)
    {
        if (match.Groups.TryGetValue(groupName, out Group? group))
        {
            return group;
        }

        throw new InvalidOperationException($"Group of name {groupName} has not been found.");
    }

    /// <summary>
    ///     Retrieves the type of commit from the match.
    /// </summary>
    /// <param name="match">The Match object containing the commit data.</param>
    /// <returns>The type of commit as a lowercase string.</returns>
    public static string GetCommitType(this Match match)
    {
        return match.GetGroup("type").Value.ToLower();
    }

    /// <summary>
    ///     Parses the match to extract the commit scope, if present.
    /// </summary>
    /// <param name="match">The Match object that contains the commit message data.</param>
    /// <returns>The commit scope as a string, or null if the scope is not present or empty.</returns>
    public static string? GetCommitScope(this Match match)
    {
        string value = match.GetGroup("scope").Value.ToLower();

        return string.IsNullOrEmpty(value) ? null : value;
    }

    /// <summary>
    ///     Extracts the commit description from a given match.
    /// </summary>
    /// <param name="match">The Match object from which to extract the commit description.</param>
    /// <returns>The description of the commit as a string.</returns>
    public static string GetCommitDescription(this Match match)
    {
        return match.GetGroup("description").Value;
    }

    /// <summary>
    ///     Retrieves the commit body from the match.
    /// </summary>
    /// <param name="match">The Match object containing the groups.</param>
    /// <returns>The commit body as a string, or null if the body is empty.</returns>
    public static string? GetCommitBody(this Match match)
    {
        string value = match.GetGroup("body").Value;

        return string.IsNullOrEmpty(value) ? null : value;
    }

    /// <summary>
    ///     Determines whether the commit includes a breaking change.
    /// </summary>
    /// <param name="match">The Match object containing the commit data.</param>
    /// <returns>True if the commit is a breaking change; otherwise, false.</returns>
    public static bool IsCommitBreakingChange(this Match match)
    {
        return match.Groups.TryGetValue("breaking", out Group? _) || match.Groups.TryGetValue("breaking_change", out Group? _);
    }

    /// <summary>
    ///     Retrieves a breaking change description from the match, if one exists.
    /// </summary>
    /// <param name="match">The Match object containing the commit data.</param>
    /// <returns>The breaking change description if present; otherwise, null.</returns>
    public static string? GetBreakingDescription(this Match match)
    {
        if (match.Groups.TryGetValue("breaking_description", out Group? description))
        {
            return string.IsNullOrEmpty(description.Value) ? null : description.Value;
        }

        return null;
    }

    /// <summary>
    ///     Retrieves the commit footer from the match.
    /// </summary>
    /// <param name="match">The Match object containing the commit data.</param>
    /// <returns>The commit footer as a string.</returns>
    public static string GetCommitFooters(this Match match)
    {
        return match.GetGroup("footers").Value;
    }
}