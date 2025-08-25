// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.RegularExpressions;

namespace Snap.Hutao.Model.Metadata.Converter;

internal static partial class CommonNameExtractor
{
    [GeneratedRegex("^UI_(.*)$")]
    private static partial Regex UI { get; }

    [GeneratedRegex("^UI_AvatarIcon_(.*)$")]
    private static partial Regex UIAvatarIcon { get; }

    public static string ExtractUIAvatarIconName(string name)
    {
        return UIAvatarIcon.Match(name).Groups[1].Value;
    }

    public static string ExtractUIName(string name)
    {
        return UI.Match(name).Groups[1].Value;
    }
}