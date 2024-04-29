// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text;

namespace Snap.Hutao.Core.Logging;

[SuppressMessage("", "SH002")]
internal static class LoggerExtension
{
    public static void LogColorizedDebug(this ILogger logger, Exception? exception, LogMessage message, params LogArgument[] args)
    {
        logger.LogColorized(LogLevel.Debug, exception, message, args);
    }

    public static void LogColorizedDebug(this ILogger logger, LogMessage message, params LogArgument[] args)
    {
        logger.LogColorized(LogLevel.Debug, message, args);
    }

    public static void LogColorizedTrace(this ILogger logger, Exception? exception, LogMessage message, params LogArgument[] args)
    {
        logger.LogColorized(LogLevel.Trace, exception, message, args);
    }

    public static void LogColorizedTrace(this ILogger logger, LogMessage message, params LogArgument[] args)
    {
        logger.LogColorized(LogLevel.Trace, message, args);
    }

    public static void LogColorizedInformation(this ILogger logger, Exception? exception, LogMessage message, params LogArgument[] args)
    {
        logger.LogColorized(LogLevel.Information, exception, message, args);
    }

    public static void LogColorizedInformation(this ILogger logger, LogMessage message, params LogArgument[] args)
    {
        logger.LogColorized(LogLevel.Information, message, args);
    }

    public static void LogColorizedWarning(this ILogger logger, Exception? exception, LogMessage message, params LogArgument[] args)
    {
        logger.LogColorized(LogLevel.Warning, exception, message, args);
    }

    public static void LogColorizedWarning(this ILogger logger, LogMessage message, params LogArgument[] args)
    {
        logger.LogColorized(LogLevel.Warning, message, args);
    }

    public static void LogColorizedError(this ILogger logger, Exception? exception, LogMessage message, params LogArgument[] args)
    {
        logger.LogColorized(LogLevel.Error, exception, message, args);
    }

    public static void LogColorizedError(this ILogger logger, LogMessage message, params LogArgument[] args)
    {
        logger.LogColorized(LogLevel.Error, message, args);
    }

    public static void LogColorizedCritical(this ILogger logger, Exception? exception, LogMessage message, params LogArgument[] args)
    {
        logger.LogColorized(LogLevel.Critical, exception, message, args);
    }

    public static void LogColorizedCritical(this ILogger logger, LogMessage message, params LogArgument[] args)
    {
        logger.LogColorized(LogLevel.Critical, message, args);
    }

    public static void LogColorized(this ILogger logger, LogLevel logLevel, LogMessage message, params LogArgument[] args)
    {
        logger.LogColorized(logLevel, 0, null, message, args);
    }

    public static void LogColorized(this ILogger logger, LogLevel logLevel, EventId eventId, LogMessage message, params LogArgument[] args)
    {
        logger.LogColorized(logLevel, eventId, null, message, args);
    }

    public static void LogColorized(this ILogger logger, LogLevel logLevel, Exception? exception, LogMessage message, params LogArgument[] args)
    {
        logger.LogColorized(logLevel, 0, exception, message, args);
    }

    public static void LogColorized(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, LogMessage message, params LogArgument[] args)
    {
        string colorizedMessage = Colorize(message, args, out object?[] outArgs)!;
        logger.Log(logLevel, eventId, exception, colorizedMessage, outArgs);
    }

    private static string? Colorize(LogMessage message, LogArgument[] args, out object?[] outArgs)
    {
        StringBuilder resultMessageBuilder = new(message.Message.Length);
        ReadOnlySpan<char> messageSpan = message.Message.AsSpan();

        // Message base colors
        ConsoleColor? messageForeground = message.ForegroundColor;
        ConsoleColor? messageBackground = message.BackgroundColor;

        if (messageForeground.HasValue)
        {
            resultMessageBuilder.Append(ConsoleVirtualTerminalSequences.FromConsoleColor(messageForeground.Value, true));
        }

        if (messageBackground.HasValue)
        {
            resultMessageBuilder.Append(ConsoleVirtualTerminalSequences.FromConsoleColor(messageBackground.Value, false));
        }

        ReadOnlySpan<LogArgument> argSpan = args.AsSpan();
        outArgs = new object?[args.Length];

        int argIndex = 0;
        for (int index = 0; index < messageSpan.Length; index++)
        {
            if (messageSpan[index] == '{')
            {
                ref readonly LogArgument arg = ref argSpan[argIndex];
                outArgs[argIndex] = arg.Argument;
                argIndex++;
                if (arg.ForegroundColor.HasValue)
                {
                    resultMessageBuilder.Append(ConsoleVirtualTerminalSequences.FromConsoleColor(arg.ForegroundColor.Value, true));
                }

                if (arg.BackgroundColor.HasValue)
                {
                    resultMessageBuilder.Append(ConsoleVirtualTerminalSequences.FromConsoleColor(arg.BackgroundColor.Value, false));
                }

                int closingIndex = messageSpan[index..].IndexOf('}');
                resultMessageBuilder.Append(messageSpan.Slice(index, closingIndex + 1));

                index += closingIndex;

                if (arg.ForegroundColor.HasValue || arg.BackgroundColor.HasValue)
                {
                    // Restore message colors
                    if (messageForeground.HasValue || messageBackground.HasValue)
                    {
                        if (messageForeground.HasValue)
                        {
                            resultMessageBuilder.Append(ConsoleVirtualTerminalSequences.FromConsoleColor(messageForeground.Value, true));
                        }

                        if (messageBackground.HasValue)
                        {
                            resultMessageBuilder.Append(ConsoleVirtualTerminalSequences.FromConsoleColor(messageBackground.Value, false));
                        }
                    }
                    else
                    {
                        resultMessageBuilder.Append(ConsoleVirtualTerminalSequences.ForegroundWhite);
                    }
                }
            }
            else
            {
                resultMessageBuilder.Append(messageSpan[index]);
            }
        }

        // Restore default colors
        if (message.ForegroundColor.HasValue || message.BackgroundColor.HasValue)
        {
            resultMessageBuilder.Append(ConsoleVirtualTerminalSequences.ForegroundWhite);
        }

        return resultMessageBuilder.ToString();
    }
}