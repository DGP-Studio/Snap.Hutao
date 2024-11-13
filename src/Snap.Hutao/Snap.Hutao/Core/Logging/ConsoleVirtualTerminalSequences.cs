// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Logging;

internal static class ConsoleVirtualTerminalSequences
{
    public const string Default = "\e[0m";
    public const string Bold = "\e[1m";

    public const string Underline = "\e[4m";

    public const string Negative = "\e[7m";

    public const string NoBold = "\e[22m";

    public const string NoUnderline = "\e[24m";

    public const string Positive = "\e[27m";

    public const string ForegroundBlack = "\e[30m";
    public const string ForegroundRed = "\e[31m";
    public const string ForegroundGreen = "\e[32m";
    public const string ForegroundYellow = "\e[33m";
    public const string ForegroundBlue = "\e[34m";
    public const string ForegroundMagenta = "\e[35m";
    public const string ForegroundCyan = "\e[36m";
    public const string ForegroundWhite = "\e[37m";
    public const string ForegroundExtended = "\e[38m";
    public const string ForegroundDefault = "\e[39m";
    public const string BackgroundBlack = "\e[40m";
    public const string BackgroundRed = "\e[41m";
    public const string BackgroundGreen = "\e[42m";
    public const string BackgroundYellow = "\e[43m";
    public const string BackgroundBlue = "\e[44m";
    public const string BackgroundMagenta = "\e[45m";
    public const string BackgroundCyan = "\e[46m";
    public const string BackgroundWhite = "\e[47m";
    public const string BackgroundExtended = "\e[48m";
    public const string BackgroundDefault = "\e[49m";

    public const string BrightForegroundBlack = "\e[1m\e[30m";
    public const string BrightForegroundRed = "\e[1m\e[31m";
    public const string BrightForegroundGreen = "\e[1m\e[32m";
    public const string BrightForegroundYellow = "\e[1m\e[33m";
    public const string BrightForegroundBlue = "\e[1m\e[34m";
    public const string BrightForegroundMagenta = "\e[1m\e[35m";
    public const string BrightForegroundCyan = "\e[1m\e[36m";
    public const string BrightForegroundWhite = "\e[1m\e[37m";
    public const string BrightBackgroundBlack = "\e[1m\e[40m";
    public const string BrightBackgroundRed = "\e[1m\e[41m";
    public const string BrightBackgroundGreen = "\e[1m\e[42m";
    public const string BrightBackgroundYellow = "\e[1m\e[43m";
    public const string BrightBackgroundBlue = "\e[1m\e[44m";
    public const string BrightBackgroundMagenta = "\e[1m\e[45m";
    public const string BrightBackgroundCyan = "\e[1m\e[46m";
    public const string BrightBackgroundWhite = "\e[1m\e[47m";

    public const string Dim = "\e[2m";
    public const string Italic = "\e[3m";

    public const string Blink = "\e[5m";

    public const string Hidden = "\e[8m";
    public const string StrikeThrough = "\e[9m";
    public const string DoubleUnderline = "\e[21m";

    public const string NoItalic = "\e[23m";

    public const string NoBlink = "\e[25m";

    public const string NoHidden = "\e[28m";
    public const string NoStrikeThrough = "\e[29m";

    public static string FromConsoleColor(ConsoleColor color, bool foreground)
    {
        return (foreground, color) switch
        {
            (true, ConsoleColor.Black) => ForegroundBlack,
            (true, ConsoleColor.DarkBlue) => ForegroundBlue,
            (true, ConsoleColor.DarkGreen) => ForegroundGreen,
            (true, ConsoleColor.DarkCyan) => ForegroundCyan,
            (true, ConsoleColor.DarkRed) => ForegroundRed,
            (true, ConsoleColor.DarkMagenta) => ForegroundMagenta,
            (true, ConsoleColor.DarkYellow) => ForegroundYellow,
            (true, ConsoleColor.DarkGray) => BrightForegroundBlack,
            (true, ConsoleColor.Gray) => ForegroundWhite,
            (true, ConsoleColor.Blue) => BrightForegroundBlue,
            (true, ConsoleColor.Green) => BrightForegroundGreen,
            (true, ConsoleColor.Cyan) => BrightForegroundCyan,
            (true, ConsoleColor.Red) => BrightForegroundRed,
            (true, ConsoleColor.Magenta) => BrightForegroundMagenta,
            (true, ConsoleColor.Yellow) => BrightForegroundYellow,
            (true, ConsoleColor.White) => BrightForegroundWhite,
            (false, ConsoleColor.Black) => BackgroundBlack,
            (false, ConsoleColor.DarkBlue) => BackgroundBlue,
            (false, ConsoleColor.DarkGreen) => BackgroundGreen,
            (false, ConsoleColor.DarkCyan) => BackgroundCyan,
            (false, ConsoleColor.DarkRed) => BackgroundRed,
            (false, ConsoleColor.DarkMagenta) => BackgroundMagenta,
            (false, ConsoleColor.DarkYellow) => BackgroundYellow,
            (false, ConsoleColor.DarkGray) => BrightBackgroundBlack,
            (false, ConsoleColor.Gray) => BackgroundWhite,
            (false, ConsoleColor.Blue) => BrightBackgroundBlue,
            (false, ConsoleColor.Green) => BrightBackgroundGreen,
            (false, ConsoleColor.Cyan) => BrightBackgroundCyan,
            (false, ConsoleColor.Red) => BrightBackgroundRed,
            (false, ConsoleColor.Magenta) => BrightBackgroundMagenta,
            (false, ConsoleColor.Yellow) => BrightBackgroundYellow,
            (false, ConsoleColor.White) => BrightBackgroundWhite,
            _ => string.Empty,
        };
    }
}