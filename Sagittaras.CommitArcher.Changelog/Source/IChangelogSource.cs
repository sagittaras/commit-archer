namespace Sagittaras.CommitArcher.Changelog.Source;

/// <summary>
///     Describes a service that is accessing a git repository for reading
///     the commits that should be part of the Changelog.
/// </summary>
public interface IChangelogSource
{
    /// <summary>
    ///     The version that has been resolved from the repository.
    /// </summary>
    /// <exception cref="InvalidOperationException">The version has not been resolved yet.</exception>
    string ResolvedVersion { get; }
    
    /// <summary>
    ///     Resolves the latest available version from commits.
    /// </summary>
    /// <returns>String describing the latest resolved version.</returns>
    /// <exception cref="InvalidOperationException">The latest version could not be determined from the commits in repository.</exception>
    Task<string> ResolveLatestVersionAsync();

    /// <summary>
    ///     Tries to find a beginning of the set version in the commits.
    /// </summary>
    /// <param name="version">Expected version to be determined.</param>
    /// <returns>Task representing the asynchronous operation of resolving the version.</returns>
    /// <exception cref="InvalidOperationException">The version could not be found in the repository's commits.</exception>
    Task ResolveVersionAsync(string version);

    /// <summary>
    ///     Reads all the commits that are part of the resolved version.
    /// </summary>
    /// <remarks>
    ///     If the version has not been resolved yet, the latest version is determined and used.
    /// </remarks>
    /// <returns>Result describing all the changes available in the scope of the version.</returns>
    Task<IChangelogResult> GetChangelogAsync();
}