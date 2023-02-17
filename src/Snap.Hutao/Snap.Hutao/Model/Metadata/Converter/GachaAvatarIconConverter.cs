// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 立绘图标转换器
/// </summary>
[HighQuality]
internal sealed class GachaAvatarIconConverter : ValueConverter<string, Uri>
{
    /// <summary>
    /// 名称转Uri
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns>链接</returns>
    public static Uri IconNameToUri(string name)
    {
        string icon = name["UI_AvatarIcon_".Length..];
        return Web.HutaoEndpoints.StaticFile("GachaAvatarIcon", $"UI_Gacha_AvatarIcon_{icon}.png").ToUri();
    }

    /// <inheritdoc/>
    public override Uri Convert(string from)
    {
        return IconNameToUri(from);
    }
}