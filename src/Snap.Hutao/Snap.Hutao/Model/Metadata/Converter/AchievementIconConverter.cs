// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 角色头像转换器
/// </summary>
[HighQuality]
internal sealed class AchievementIconConverter : ValueConverter<string, Uri>
{
    /// <summary>
    /// 名称转Uri
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns>链接</returns>
    public static Uri IconNameToUri(string name)
    {
        return Web.HutaoEndpoints.StaticRaw("AchievementIcon", $"{name}.png").ToUri();
    }

    /// <inheritdoc/>
    public override Uri Convert(string from)
    {
        return IconNameToUri(from);
    }
}