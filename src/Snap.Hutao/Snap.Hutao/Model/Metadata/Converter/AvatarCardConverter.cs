// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 角色卡片转换器
/// </summary>
internal class AvatarCardConverter : ValueConverterBase<string, Uri>
{
    private const string BaseUrl = "https://static.snapgenshin.com/AvatarCard/{0}_Card.png";

    private static readonly Uri UIAvatarIconCostumeCard = new("https://static.snapgenshin.com/AvatarCard/UI_AvatarIcon_Costume_Card.png");

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

        return new Uri(string.Format(BaseUrl, name));
    }

    /// <inheritdoc/>
    public override Uri Convert(string from)
    {
        return IconNameToUri(from);
    }
}