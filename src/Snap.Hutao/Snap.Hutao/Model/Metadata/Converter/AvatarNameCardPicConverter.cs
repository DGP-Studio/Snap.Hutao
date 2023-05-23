// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 角色名片转换器
/// </summary>
[HighQuality]
internal sealed class AvatarNameCardPicConverter : ValueConverter<Avatar.Avatar?, Uri>
{
    /// <summary>
    /// 从角色转换到名片
    /// </summary>
    /// <param name="avatar">角色</param>
    /// <returns>名片</returns>
    public static Uri AvatarToUri(Avatar.Avatar? avatar)
    {
        if (avatar == null)
        {
            return null!;
        }

        string avatarName = ReplaceSpecialCaseNaming(avatar.Icon["UI_AvatarIcon_".Length..]);
        return Web.HutaoEndpoints.StaticFile("NameCardPic", $"UI_NameCardPic_{avatarName}_P.png").ToUri();
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