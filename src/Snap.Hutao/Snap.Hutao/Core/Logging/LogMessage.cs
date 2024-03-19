// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Logging;

internal readonly struct LogMessage
{
    public readonly string Message;
    public readonly ConsoleColor? ForegroundColor;
    public readonly ConsoleColor? BackgroundColor;

    public LogMessage(string message, ConsoleColor? foreground = default, ConsoleColor? background = default)
    {
        Message = message;
        ForegroundColor = foreground;
        BackgroundColor = background;
    }

    public static implicit operator LogMessage(string value)
    {
        return new(value);
    }

    public static implicit operator LogMessage((string Value, ConsoleColor? Foreground) tuple)
    {
        return new(tuple.Value, tuple.Foreground);
    }
}