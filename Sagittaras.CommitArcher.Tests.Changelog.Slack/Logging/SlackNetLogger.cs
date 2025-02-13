using Microsoft.Extensions.Logging;
using SlackNet;
using ILogger = SlackNet.ILogger;

namespace Sagittaras.CommitArcher.Tests.Changelog.Slack.Logging;

/// <summary>
///     Provides a logging implementation for SlackNet utilizing Microsoft.Extensions.Logging.
/// </summary>
/// <remarks>
///     This logger maps various SlackNet log categories to corresponding .NET log levels
///     and forwards the log messages to the provided logger instance.
/// </remarks>
public class SlackNetLogger(ILogger<SlackNetLogger> logger) : ILogger
{
    /// <inheritdoc />
    public void Log(ILogEvent logEvent)
    {
        if (logEvent.Exception is SlackException slackException)
        {
            HandleSlackException(slackException);
            return;
        }

        LogLevel level = logEvent.Category switch
        {
            LogCategory.Data => LogLevel.None,
            LogCategory.Serialization => LogLevel.None,
            LogCategory.Internal => LogLevel.Debug,
            LogCategory.Request => LogLevel.Information,
            LogCategory.Error => LogLevel.Error,
            _ => throw new ArgumentOutOfRangeException(nameof(logEvent.Category), logEvent.Category, null)
        };

        if (level == LogLevel.None)
        {
            // Meziantou's extensions does not support level none.
            return;
        }

        // In this case, we have a complete message template.
        // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
#pragma warning disable CA2254
        logger.Log(level, logEvent.Exception, logEvent.MessageTemplate, logEvent.Properties.Values.ToArray());
#pragma warning restore CA2254
    }

    /// <summary>
    ///     Handles exceptions specific to Slack communication.
    /// </summary>
    /// <remarks>
    ///     Logs the error code and any additional error messages if available.
    /// </remarks>
    /// <param name="exception">The SlackException object containing error details, including codes and messages.</param>
    private void HandleSlackException(SlackException exception)
    {
        logger.LogError("Slack error code [{Code}] occured, read additional messages below if available", exception.ErrorCode);
        foreach (string message in exception.ErrorMessages)
        {
            logger.LogError("{Message}", message);
        }
    }
}