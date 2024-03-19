// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Logging;

internal static class ConsoleVirtualTerminalSequences
{
    public const string Default = "\u001b[0m";
    public const string Bold = "\u001b[1m";

    public const string Underline = "\u001b[4m";

    public const string Negative = "\u001b[7m";

    public const string NoBold = "\u001b[22m";

    public const string NoUnderline = "\u001b[24m";

    public const string Positive = "\u001b[27m";

    public const string ForegroundBlack = "\u001b[30m";
    public const string ForegroundRed = "\u001b[31m";
    public const string ForegroundGreen = "\u001b[32m";
    public const string ForegroundYellow = "\u001b[33m";
    public const string ForegroundBlue = "\u001b[34m";
    public const string ForegroundMagenta = "\u001b[35m";
    public const string ForegroundCyan = "\u001b[36m";
    public const string ForegroundWhite = "\u001b[37m";
    public const string ForegroundExtended = "\u001b[38m";
    public const string ForegroundDefault = "\u001b[39m";
    public const string BackgroundBlack = "\u001b[40m";
    public const string BackgroundRed = "\u001b[41m";
    public const string BackgroundGreen = "\u001b[42m";
    public const string BackgroundYellow = "\u001b[43m";
    public const string BackgroundBlue = "\u001b[44m";
    public const string BackgroundMagenta = "\u001b[45m";
    public const string BackgroundCyan = "\u001b[46m";
    public const string BackgroundWhite = "\u001b[47m";
    public const string BackgroundExtended = "\u001b[48m";
    public const string BackgroundDefault = "\u001b[49m";

    public const string BrightForegroundBlack = "\u001b[90m";
    public const string BrightForegroundRed = "\u001b[91m";
    public const string BrightForegroundGreen = "\u001b[92m";
    public const string BrightForegroundYellow = "\u001b[93m";
    public const string BrightForegroundBlue = "\u001b[94m";
    public const string BrightForegroundMagenta = "\u001b[95m";
    public const string BrightForegroundCyan = "\u001b[96m";
    public const string BrightForegroundWhite = "\u001b[97m";
    public const string BrightBackgroundBlack = "\u001b[100m";
    public const string BrightBackgroundRed = "\u001b[101m";
    public const string BrightBackgroundGreen = "\u001b[102m";
    public const string BrightBackgroundYellow = "\u001b[103m";
    public const string BrightBackgroundBlue = "\u001b[104m";
    public const string BrightBackgroundMagenta = "\u001b[105m";
    public const string BrightBackgroundCyan = "\u001b[106m";
    public const string BrightBackgroundWhite = "\u001b[107m";

    public const string Dim = "\u001b[2m";
    public const string Italic = "\u001b[3m";

    public const string Blink = "\u001b[5m";

    public const string Hidden = "\u001b[8m";
    public const string StrikeThrough = "\u001b[9m";
    public const string DoubleUnderline = "\u001b[21m";

    public const string NoItalic = "\u001b[23m";

    public const string NoBlink = "\u001b[25m";

    public const string NoHidden = "\u001b[28m";
    public const string NoStrikeThrough = "\u001b[29m";

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