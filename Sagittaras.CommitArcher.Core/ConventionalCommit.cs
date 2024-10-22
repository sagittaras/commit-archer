namespace Sagittaras.CommitArcher.Core;

/// <summary>
///     Represents a conventional commit message according to the Conventional Commits specification.
/// </summary>
public class ConventionalCommit : IConventionalCommit
{
    /// <inheritdoc />
    public string Type { get; set; } = string.Empty;

    /// <inheritdoc />
    public string? Scope { get; set; }

    /// <inheritdoc />
    public string Description { get; set; } = string.Empty;

    /// <inheritdoc />
    public string? Body { get; set; }

    /// <inheritdoc />
    public bool IsBreakingChange { get; set; }

    /// <inheritdoc />
    public string? BreakingDescription { get; set; }

    /// <inheritdoc />
    public IDictionary<string, string> Footers { get; set; } = new Dictionary<string, string>();
}