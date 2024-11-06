using Moq;
using Sagittaras.CommitArcher.Changelog.Source;
using Sagittaras.CommitArcher.Core;

namespace Sagittaras.CommitArcher.Tests.Changelog.Slack;

public static class ChangelogSourceMock
{
    private static ChangelogResult Result
    {
        get
        {
            ChangelogResult result = new()
            {
                Version = "1.0.0",
                VersionDescription = "Optional description for the changelog.",
                Commits = [
                    new ConventionalCommit
                    {
                        Type = "feat",
                        Description = "A new changelog package has been released.",
                        Body = "Sounds great, isn't it?"
                    },
                    new ConventionalCommit
                    {
                        Type = "fix",
                        Description = "Yet it needed to be fixed."
                    }
                ]
            };

            return result;
        }
    } 
    
    public static IChangelogSource Create()
    {
        Mock<IChangelogSource> mock = new();
        mock.Setup(x => x.GetLatestChangelogAsync())
            .ReturnsAsync(Result)
            ;

        return mock.Object;
    }
}