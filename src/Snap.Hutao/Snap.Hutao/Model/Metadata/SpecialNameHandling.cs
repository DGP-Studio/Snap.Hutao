// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Model.Metadata;

internal static partial class SpecialNameHandling
{
    [GeneratedRegex("\\{M#(.*?)\\}\\{F#(.*?)\\}")]
    private static partial Regex MaleFemaleRegex { get; }

    [GeneratedRegex("\\{F#(.*?)\\}\\{M#(.*?)\\}")]
    private static partial Regex FemaleMaleRegex { get; }

    [GeneratedRegex("\\{LAYOUT_MOBILE#(.+?)\\}\\{LAYOUT_PC#(.+?)\\}\\{LAYOUT_PS#(.+?)\\}")]
    private static partial Regex LayoutRegex { get; }

    // Use this regex to query special names in metadata
    // "#(?!.*(?:[A-Z0-9]{8}|SPACE|F#|M#|NON_BREAK_SPACE|REALNAME\[ID\(1\)(\|HOSTONLY\(true\)|)\]|\{INPUT_ACTION_TYPE|\{LAYOUT_MOBILE#.+?\}|\{LAYOUT_PC#.+?\}|\{LAYOUT_PS#.+?\})).*
    public static string Handle(string input)
    {
        if (string.IsNullOrEmpty(input) || input.AsSpan()[0] is not '#')
        {
            return input;
        }

        // {PLAYERAVATAR#SEXPRO[INFO_MALE_PRONOUN_BROTHER|INFO_FEMALE_PRONOUN_SISTERA]}
        // {SPACE}
        // {INPUT_ACTION_TYPE#??}
        StringBuilder resultBuilder = new(input);
        resultBuilder
            .Replace("{MATEAVATAR#SEXPRO[INFO_MALE_PRONOUN_BOYFIRST|INFO_FEMALE_PRONOUN_GIRLFIRST]}", SH.MetadataSpecialNameMetaAvatarSexProInfoPronounBoyGirlFirst)
            .Replace("{MATEAVATAR#SEXPRO[INFO_MALE_PRONOUN_BOYD|INFO_FEMALE_PRONOUN_GIRLD]}", SH.MetadataSpecialNameMetaAvatarSexProInfoPronounBoyGirlD)
            .Replace("{PLAYERAVATAR#SEXPRO[INFO_MALE_PRONOUN_HE|INFO_FEMALE_PRONOUN_SHE]}", SH.MetadataSpecialNamePlayerAvatarSexProInfoPronounHeShe)
            .Replace("{REALNAME[ID(1)|HOSTONLY(true)]}", SH.MetadataSpecialNameRealNameId1)
            .Replace("{REALNAME[ID(1)]}", SH.MetadataSpecialNameRealNameId1)
            .Replace("{TIMEZONE}", SH.MetadataSpecialNameServerTimeZone)
            .Replace("{NICKNAME}", SH.MetadataSpecialNameNickname)
            .Replace("{NON_BREAK_SPACE}", "\u00A0");

        input = resultBuilder.ToString();

        input = MaleFemaleRegex.Replace(input, "<color=#1E90FF>$1</color>/<color=#FFB6C1>$2</color>");
        input = FemaleMaleRegex.Replace(input, "<color=#FFB6C1>$1</color>/<color=#1E90FF>$2</color>");
        input = LayoutRegex.Replace(input, "$2");

        return input[1..];
    }
}