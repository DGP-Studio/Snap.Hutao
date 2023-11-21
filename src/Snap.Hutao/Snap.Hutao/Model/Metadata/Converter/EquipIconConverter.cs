// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 武器图片转换器
/// </summary>
[HighQuality]
internal sealed class EquipIconConverter : ValueConverter<string, Uri>
{
    /// <summary>
    /// 名称转Uri
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns>链接</returns>
    public static Uri IconNameToUri(string name)
    {
        return Web.HutaoEndpoints.StaticRaw("EquipIcon", $"{name}.png").ToUri();
    }

    /// <inheritdoc/>
    public override Uri Convert(string from)
    {
        return IconNameToUri(from);
    }
}