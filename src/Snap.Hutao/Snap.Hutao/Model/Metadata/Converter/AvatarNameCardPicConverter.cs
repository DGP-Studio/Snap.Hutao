// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 角色名片转换器
/// </summary>
internal class AvatarNameCardPicConverter : ValueConverterBase<Avatar.Avatar?, Uri>
{
    private const string BaseUrl = "https://static.snapgenshin.com/NameCardPic/UI_NameCardPic_{0}_P.png";

    /// <inheritdoc/>
    public override Uri Convert(Avatar.Avatar? avatar)
    {
        if (avatar == null)
        {
            return null!;
        }

        string avatarName = ReplaceSpecialCaseNaming(avatar.Icon["UI_AvatarIcon_".Length..]);
        return new Uri(string.Format(BaseUrl, avatarName));
    }

    private static string ReplaceSpecialCaseNaming(string avatarName)
    {
        return avatarName switch
        {
            "Yae" => "Yae1",
            _ => avatarName,
        };
    }
}