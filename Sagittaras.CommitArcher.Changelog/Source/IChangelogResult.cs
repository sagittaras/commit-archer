using Sagittaras.CommitArcher.Core;

namespace Sagittaras.CommitArcher.Changelog.Source;

/// <summary>
///     Represents a result of reading the repository for the latest changelog.
/// </summary>
public interface IChangelogResult
{
    /// <summary>
    ///     Gets the version string representing the latest changelog version in the repository.
    /// </summary>
    string Version { get; }

    /// <summary>
    ///     Gets the description of the version, which provides additional contextual information about the latest changes.
    /// </summary>
    string VersionDescription { get; }
    
    /// <summary>
    ///     Instance of the commit which is marking the release.
    /// </summary>
    IConventionalCommit ReleaseCommit { get; }

    /// <summary>
    ///     Gets a collection of conventional commits extracted from the repository.
    /// </summary>
    /// <remarks>
    ///     Contains all commits that are part of the current version's changelog.
    /// </remarks>
    IReadOnlyCollection<IConventionalCommit> Commits { get; }
}