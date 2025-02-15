﻿using Sagittaras.CommitArcher.Core;

namespace Sagittaras.CommitArcher.Changelog.Source;

/// <summary>
///     Base implementation of the <see cref="IChangelogResult"/> interface.
/// </summary>
public class ChangelogResult : IChangelogResult
{
    /// <inheritdoc />
    public string Version { get; set; } = "0.0.0";

    /// <inheritdoc />
    public string VersionDescription { get; set; } = string.Empty;

    /// <inheritdoc />
    public IConventionalCommit ReleaseCommit { get; set; } = new ConventionalCommit();

    /// <inheritdoc />
    public IReadOnlyCollection<IConventionalCommit> Commits { get; set; } = new List<IConventionalCommit>();
}