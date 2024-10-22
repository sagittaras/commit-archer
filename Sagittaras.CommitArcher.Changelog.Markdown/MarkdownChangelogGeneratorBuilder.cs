namespace Sagittaras.CommitArcher.Changelog.Markdown;

/// <summary>
///     Provides a builder for creating instances of <see cref="MarkdownChangelogGenerator"/>.
/// </summary>
public class MarkdownChangelogGeneratorBuilder
{
    /// <summary>
    ///     Gets or sets the source for reading commits that should be included in the changelog.
    /// </summary>
    internal IChangelogSource Source { get; set; } = null!;

    /// <summary>
    ///     Contains the mapping of commit types to their corresponding headings in the changelog.
    /// </summary>
    /// <remarks>
    ///     Types that are not part of the dictionary won't be listened in the changelog.
    /// </remarks>
    internal Dictionary<string, string> CommitTypes { get; } = new()
    {
        {"feat", "\u2728 Features"},
        {"fix", "\ud83d\udc1b Fixes"}
    };

    /// <summary>
    ///     Sets the source for the changelog by providing an instance of <see cref="IChangelogSource"/>.
    /// </summary>
    /// <param name="source">The source that will be used to generate the changelog.</param>
    /// <returns>An instance of <see cref="MarkdownChangelogGeneratorBuilder"/> for chaining methods.</returns>
    public MarkdownChangelogGeneratorBuilder UseSource(IChangelogSource source)
    {
        Source = source;
        return this;
    }

    /// <summary>
    ///     Adds a new commit type with its corresponding heading label to the changelog.
    /// </summary>
    /// <remarks>
    ///     Types that are not part of the dictionary won't be listened in the changelog.
    /// </remarks>
    /// <param name="type">The type of commit to add, for example, "feat" or "fix".</param>
    /// <param name="headingLabel">The heading label associated with the commit type, which will be used in the changelog.</param>
    /// <returns>An instance of <see cref="MarkdownChangelogGeneratorBuilder"/> for chaining methods.</returns>
    public MarkdownChangelogGeneratorBuilder AddCommitType(string type, string headingLabel)
    {
        CommitTypes.Add(type, headingLabel);
        return this;
    }

    /// <summary>
    ///     Constructs an instance of <see cref="MarkdownChangelogGenerator"/> based on the current configuration of the builder.
    /// </summary>
    /// <returns>An instance of <see cref="MarkdownChangelogGenerator"/> for generating changelogs in Markdown format.</returns>
    public MarkdownChangelogGenerator Build()
    {
        return new MarkdownChangelogGenerator(this);
    }
}