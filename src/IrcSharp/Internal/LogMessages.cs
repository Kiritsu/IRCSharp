using Microsoft.Extensions.Logging;

namespace IrcSharp.Internal;

internal static partial class LogMessages
{
    [LoggerMessage(1, LogLevel.Error, "Error while invoking handler")]
    internal static partial void LogErrorWhileInvokingHandler(this ILogger<IrcClient> logger, Exception ex);

    [LoggerMessage(2, LogLevel.Error, "Failed to connect to the server")]
    internal static partial void LogFailedToConnectToTheServer(this ILogger<IrcClient> logger, Exception ex);
}