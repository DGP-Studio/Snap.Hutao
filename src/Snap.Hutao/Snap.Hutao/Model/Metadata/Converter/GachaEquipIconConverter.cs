// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 武器祈愿图片转换器
/// </summary>
[HighQuality]
internal sealed class GachaEquipIconConverter : ValueConverter<string, Uri>
{
    /// <summary>
    /// 名称转Uri
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns>链接</returns>
    public static Uri IconNameToUri(string name)
    {
        string icon = name["UI_".Length..];
        return new Uri(Web.HutaoEndpoints.StaticRaw("GachaEquipIcon", $"UI_Gacha_{icon}.png"));
    }

    /// <inheritdoc/>
    public override Uri Convert(string from)
    {
        return IconNameToUri(from);
    }
}