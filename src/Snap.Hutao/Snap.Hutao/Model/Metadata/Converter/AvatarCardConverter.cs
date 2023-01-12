// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 角色卡片转换器
/// </summary>
internal class AvatarCardConverter : ValueConverter<string, Uri>
{
    private const string CostumeCard = "UI_AvatarIcon_Costume_Card.png";
    private static readonly Uri UIAvatarIconCostumeCard = new(Web.HutaoEndpoints.StaticFile("AvatarCard", CostumeCard));

    /// <summary>
    /// 名称转Uri
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns>链接</returns>
    public static Uri IconNameToUri(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return UIAvatarIconCostumeCard;
        }

        return new Uri(Web.HutaoEndpoints.StaticFile("AvatarCard", $"{name}_Card.png"));
    }

    /// <inheritdoc/>
    public override Uri Convert(string from)
    {
        return IconNameToUri(from);
    }
}