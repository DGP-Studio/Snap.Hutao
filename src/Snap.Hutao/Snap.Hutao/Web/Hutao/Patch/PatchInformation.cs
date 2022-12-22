// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Patch;

/// <summary>
/// 更新信息
/// </summary>
public class PatchInformation
{
    /// <summary>
    /// 标签名 通常被替换为版本
    /// </summary>
    public string TagName { get; set; } = default!;

    /// <summary>
    /// 更新日志
    /// </summary>
    public string Body { get; set; } = default!;

    /// <summary>
    /// 浏览器下载链接
    /// </summary>
    public Uri BrowserDownloadUrl { get; set; } = default!;

    /// <summary>
    /// 缓存时间
    /// </summary>
    public DateTimeOffset CacheTime { get; set; }
}
