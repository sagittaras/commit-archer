namespace Sagittaras.CommitArcher.Core;

/// <summary>
///     Represents a commit with details about the author, committer, and repository-related information.
/// </summary>
public class Commit : ICommit
{
    /// <inheritdoc />
    public string Author { get; set; } = string.Empty;

    /// <inheritdoc />
    public string AuthorEmail { get; set; } = string.Empty;

    /// <inheritdoc />
    public DateTime Authored { get; set; } = DateTime.UtcNow;

    /// <inheritdoc />
    public string Committer { get; set; } = string.Empty;

    /// <inheritdoc />
    public string CommitterEmail { get; set; } = string.Empty;

    /// <inheritdoc />
    public DateTime Committed { get; set; } = DateTime.UtcNow;

    /// <inheritdoc />
    public string Sha { get; set; } = string.Empty;

    /// <inheritdoc />
    public string Url { get; set; } = string.Empty;
}