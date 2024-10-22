using System.Text;
using Sagittaras.CommitArcher.Core;

namespace Sagittaras.CommitArcher.Changelog.Markdown;

/// <summary>
///     Generates the changelog as a string in Markdown format.
/// </summary>
public class MarkdownChangelogGenerator
{
    /// <summary>
    ///     A builder for configuring and obtaining a MarkdownChangelogGenerator instance.
    /// </summary>
    private readonly MarkdownChangelogGeneratorBuilder _builder;

    /// <summary>
    ///     Generates the changelog as a string in Markdown format.
    /// </summary>
    internal MarkdownChangelogGenerator(MarkdownChangelogGeneratorBuilder builder)
    {
        _builder = builder;
    }

    /// <summary>
    ///     Generates the changelog document asynchronously in Markdown format.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, with a result of the generated changelog in Markdown format.</returns>
    public async Task<string> GenerateAsync()
    {
        IChangelogResult result = await _builder.Source.GetLatestChangelogAsync();
        StringBuilder builder = new();
        builder.AppendLine($"# \ud83d\ude80 Version {result.Version}");
        if (!string.IsNullOrEmpty(result.VersionDescription))
        {
            builder.AppendLine($"*{result.VersionDescription}*");
        }

        foreach ((string type, string heading) in _builder.CommitTypes)
        {
            builder.AppendLine();
            builder.AppendLine($"## {heading}");
            foreach (IConventionalCommit commit in result.Commits.Where(x => x.Type == type))
            {
                builder.AppendLine($"- **{commit.Description}**");
                if (!string.IsNullOrEmpty(commit.Body))
                {
                    builder.AppendLine($"\t- _{commit.Body}_");
                }
            }
        }
        
        return builder.ToString();
    }
}