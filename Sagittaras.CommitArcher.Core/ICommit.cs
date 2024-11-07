namespace Sagittaras.CommitArcher.Core;

/// <summary>
///     Describes some attributes that can be read from the original commit. 
/// </summary>
public interface ICommit
{
    /// <summary>
    ///     Person who is author of the changes in the commit.
    /// </summary>
    string Author { get; }
    
    /// <summary>
    ///     Email of the author of the changes.
    /// </summary>
    string AuthorEmail { get; }
    
    /// <summary>
    ///     Date & time when the changes have been made.
    /// </summary>
    /// <remarks>
    ///     In UTC format.
    /// </remarks>
    DateTime Authored { get; }
    
    /// <summary>
    ///     Person who created the commit.
    /// </summary>
    string Committer { get; }
    
    /// <summary>
    ///     Email of the committer.
    /// </summary>
    string CommitterEmail { get; }
    
    /// <summary>
    ///     Date & Time when the changes have been committed.
    /// </summary>
    /// <remarks>
    ///     In UTC format.
    /// </remarks>
    DateTime Committed { get; }

    /// <summary>
    ///     The SHA hash that uniquely identifies the commit.
    /// </summary>
    string Sha { get; }

    /// <summary>
    ///     URL associated with the commit.
    /// </summary>
    /// <remarks>
    ///     Typically pointing to the commit detail page in the repository hosting service.
    /// </remarks>
    string Url { get; }
}