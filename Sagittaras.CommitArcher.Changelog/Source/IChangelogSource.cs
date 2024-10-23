namespace Sagittaras.CommitArcher.Changelog.Source;

/// <summary>
///     Describes a service that is accessing a git repository for reading
///     the commits that should be part of the Changelog.
/// </summary>
public interface IChangelogSource
{
    /// <summary>
    ///     Reads the commits for changelog from target repository.
    /// </summary>
    /// <returns>Collection of commits in conventional format.</returns>
    Task<IChangelogResult> GetLatestChangelogAsync();
}