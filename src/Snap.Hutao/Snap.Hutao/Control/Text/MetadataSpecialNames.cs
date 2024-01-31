// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Control.Text;

internal static partial class MetadataSpecialNames
{
    public static string Handle(string input)
    {
        if (input.AsSpan()[0] is not '#')
        {
            return input;
        }

        StringBuilder resultBuilder = new(input);
        resultBuilder
            .Replace("{MATEAVATAR#SEXPRO[INFO_MALE_PRONOUN_BOYD|INFO_FEMALE_PRONOUN_GIRLD]}", SH.ControlTextMetadataSpecialNameMetaAvatarSexProD)
            .Replace("{PLAYERAVATAR#SEXPRO[INFO_MALE_PRONOUN_HE|INFO_FEMALE_PRONOUN_SHE]}", SH.ControlTextMetadataSpecialNamePlayerAvatarSexPro)
            .Replace("{REALNAME[ID(1)]}", SH.ControlTextMetadataSpecialNameRealNameId1);

        input = resultBuilder.ToString();

        // {M#.}{F#.}
        input = MaleFemaleRegex().Replace(input, SH.ControlTextMetadataSpecialNameMaleFemale);

        return input[1..];
    }

    [GeneratedRegex("\\{M#(.*?)\\}\\{F#(.*?)\\}")]
    private static partial Regex MaleFemaleRegex();
}