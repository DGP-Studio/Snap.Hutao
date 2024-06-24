// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Data.Converter;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 角色卡片转换器
/// </summary>
[HighQuality]
internal sealed class AvatarCardConverter : ValueConverter<string, Uri>, IIconNameToUriConverter
{
    private const string CostumeCard = "UI_AvatarIcon_Costume_Card.png";
    private static readonly Uri UIAvatarIconCostumeCard = Web.HutaoEndpoints.StaticRaw("AvatarCard", CostumeCard).ToUri();

    /// <summary>
    /// 名称转Uri
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns>链接</returns>
    public static Uri IconNameToUri(string name)
    {
        return string.IsNullOrEmpty(name)
            ? UIAvatarIconCostumeCard
            : Web.HutaoEndpoints.StaticRaw("AvatarCard", $"{name}_Card.png").ToUri();
    }

    /// <inheritdoc/>
    public override Uri Convert(string from)
    {
        return IconNameToUri(from);
    }
}