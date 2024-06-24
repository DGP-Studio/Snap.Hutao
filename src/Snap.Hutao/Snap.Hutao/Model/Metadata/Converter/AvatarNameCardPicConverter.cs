// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Data.Converter;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 角色名片转换器
/// </summary>
[HighQuality]
internal sealed class AvatarNameCardPicConverter : ValueConverter<Avatar.Avatar?, Uri>
{
    public static Uri AvatarToUri(Avatar.Avatar? avatar)
    {
        if (avatar is null)
        {
            return default!;
        }

        string avatarName = ReplaceSpecialCaseNaming(avatar.Icon["UI_AvatarIcon_".Length..]);
        return Web.HutaoEndpoints.StaticRaw("NameCardPic", $"UI_NameCardPic_{avatarName}_P.png").ToUri();
    }

    /// <inheritdoc/>
    public override Uri Convert(Avatar.Avatar? avatar)
    {
        return AvatarToUri(avatar);
    }

    private static string ReplaceSpecialCaseNaming(string avatarName)
    {
        return avatarName switch
        {
            "Yae" => "Yae1",
            "Momoka" => "Kirara",
            _ => avatarName,
        };
    }
}