// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.RegularExpressions;

namespace Snap.Hutao.Service.Notification;

internal static partial class TextReadingTime
{
    [GeneratedRegex("""\p{IsCJKUnifiedIdeographs}""")]
    private static partial Regex ChineseRegex { get; }

    [GeneratedRegex("""\b[A-Za-z]+\b""")]
    private static partial Regex EnglishRegex { get; }

    public static int Estimate(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        double chineseSeconds = (double)ChineseRegex.Matches(text).Count / 250 * 60;
        double englishSeconds = (double)EnglishRegex.Matches(text).Count / 200 * 60;

        return (int)Math.Ceiling((chineseSeconds + englishSeconds) * 1000);
    }
}