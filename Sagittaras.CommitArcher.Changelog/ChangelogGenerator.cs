namespace Sagittaras.CommitArcher.Changelog;

/// <summary>
///     Abstract base class for generating changelogs from a collection of commits.
/// </summary>
public abstract class ChangelogGenerator
{
    /// <summary>
    ///     Contains the mapping of commit types to their corresponding headings in the changelog.
    /// </summary>
    /// <remarks>
    ///     Types that are not part of the dictionary won't be listened in the changelog.
    /// </remarks>
    public Dictionary<string, string> CommitTypes { get; set; } = new()
    {
        { "feat", "\u2728 Features" },
        { "fix", "\ud83d\udc1b Fixes" }
    };
}