// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Logging;

internal readonly struct LogArgument
{
    public readonly object? Argument;
    public readonly ConsoleColor? ForegroundColor;
    public readonly ConsoleColor? BackgroundColor;

    public LogArgument(object? argument, ConsoleColor? foreground = default, ConsoleColor? background = default)
    {
        Argument = argument;
        ForegroundColor = foreground;
        BackgroundColor = background;
    }

    public static implicit operator LogArgument(string argument)
    {
        return new(argument);
    }

    public static implicit operator LogArgument((string Argument, ConsoleColor Foreground) tuple)
    {
        return new(tuple.Argument, tuple.Foreground);
    }
}