// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 角色头像转换器
/// </summary>
internal class AchievementIconConverter : ValueConverterBase<string, Uri>
{
    private const string BaseUrl = "https://static.snapgenshin.com/AchievementIcon/{0}.png";

    /// <summary>
    /// 名称转Uri
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns>链接</returns>
    public static Uri IconNameToUri(string name)
    {
        return new Uri(string.Format(BaseUrl, name));
    }

    /// <inheritdoc/>
    public override Uri Convert(string from)
    {
        return IconNameToUri(from);
    }
}