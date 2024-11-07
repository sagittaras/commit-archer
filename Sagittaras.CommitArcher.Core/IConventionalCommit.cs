namespace Sagittaras.CommitArcher.Core;

/// <summary>
///     Represents a conventional commit, providing a standardized structure for commit messages.
/// </summary>
/// <remarks>
///     Object representation of a conventional commit specification which is described as:
///     <code>
///         &lt;type&gt;[optional scope]: &lt;description&gt;
///         <br/><br/>
///         [optional body]
///         <br/><br/>
///         [optional footer(s)]
///     </code>
///
///     The commit contains the following structural elements, to communicate intent to the consumers of your library:
///     <ul>
///         <li><b>fix</b>: a commit of the <i>type</i> <c>fix</c> patches a bug in your codebase <i>(this correlates with <c>PATCH</c> in Semantic Versioning)</i>.</li>
///         <li><b>feat</b>: a commit of the <i>type</i> <c>feat</c> introduces a new feature to the codebase <i>(this correlates with <c>MINOR</c> in Semantic Versioning)</i>.</li>
///         <li><b>BREAKING CHANGE</b>: a commit that has a footer <c>BREAKING CHANGE:</c>, or appends a <c>!</c> after the type/scope, introduces a breaking API change (correlating with <c>MAJOR</c> in Semantic Versioning). A BREAKING CHANGE can be part of commits of any <i>type</i>.</li>
///         <li><i>types</i> other than <c>fix:</c> and <c>feat:</c> are allowed, for example <c>build:</c>, <c>chore:</c>, <c>ci:</c>, <c>docs:</c>, <c>style:</c>, <c>refactor:</c>, <c>perf:</c>, <c>test:</c>, and others.</li>
///         <li><i>footers</i> other than <c>BREAKING CHANGE: &lt;description&gt;</c> may be provided and follow a convention similar to <b><see href="https://git-scm.com/docs/git-interpret-trailers">git trailer format</see></b>.</li>
///     </ul>
/// </remarks>
/// <see href="https://www.conventionalcommits.org/en/v1.0.0/"/>
public interface IConventionalCommit
{
    /// <summary>
    ///     Gets or sets the type of the conventional commit.
    /// </summary>
    /// <remarks>
    ///     The type typically describes the category of the change,
    ///     such as <c>feat</c> for a new feature, <c>fix</c> for a bug fix,
    ///     or <c>chore</c> for changes that do not affect the code directly,
    ///     like build process or documentation changes.
    /// </remarks>
    string Type { get; }

    /// <summary>
    ///     Gets or sets the scope of the conventional commit.
    /// </summary>
    /// <remarks>
    ///     The scope provides additional contextual information about the change,
    ///     typically indicating the specific area or component of the project that is affected.
    /// </remarks>
    /// <example>The scope might denote a module or feature impacted by the commit.</example>
    string? Scope { get; }

    /// <summary>
    ///     Gets or sets the description of the conventional commit.
    /// </summary>
    /// <remarks>
    ///     The description provides a concise explanation of the change made in the commit.
    /// </remarks>
    string Description { get; }

    /// <summary>
    ///     Gets or sets the body content of the conventional commit message.
    /// </summary>
    /// <remarks>
    ///     The body typically provides a more detailed description of the commit,
    ///     elaborating on the changes made in a way that complements the short
    ///     description. This can include implementation details, reasoning behind
    ///     the change, and any other relevant information.
    /// </remarks>
    string? Body { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the commit introduces a breaking change.
    /// </summary>
    /// <remarks>
    ///     A breaking change is a modification that can potentially cause existing code to fail or act differently.
    ///     When <c>true</c>, the change requires additional attention from the developers to accommodate the modification.
    /// </remarks>
    bool IsBreakingChange { get; }

    /// <summary>
    ///     Gets or sets the description of a breaking change in the commit.
    /// </summary>
    /// <remarks>
    ///     This property provides details about the nature of the breaking change
    ///     when <c>IsBreaking</c> is set to <c>true</c>. It should clearly explain
    ///     what behavior has been altered and why it is not backward compatible.
    /// </remarks>
    string? BreakingDescription { get; }

    /// <summary>
    ///     Gets or sets the footers of the conventional commit.
    /// </summary>
    /// <remarks>
    ///     Footers are typically additional metadata at the end of a commit message.
    ///     They can include information such as issue references, co-authors, or other
    ///     custom tags that provide more context or details about the commit.
    /// </remarks>
    /// <seealso href="https://git-scm.com/docs/git-interpret-trailers"/>
    IDictionary<string, string> Footers { get; }

    /// <summary>
    ///     Gets the original commit associated with this conventional commit.
    /// </summary>
    /// <remarks>
    ///     This property provides access to the underlying commit data that this
    ///     conventional commit is based on, allowing for tracking and referencing
    ///     the original commit details.
    /// </remarks>
    ICommit OriginalCommit { get; set; }
}