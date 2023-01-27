// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package;

/// <summary>
/// 包版本项
/// </summary>
internal class VersionItem
{
    /// <summary>
    /// 服务器上的名称
    /// </summary>
    [JsonPropertyName("remoteName")]
    public string RemoteName { get; set; } = default!;

    /// <summary>
    /// MD5校验值
    /// </summary>
    [JsonPropertyName("md5")]
    public string Md5 { get; set; } = default!;

    /// <summary>
    /// 文件尺寸
    /// </summary>
    [JsonPropertyName("fileSize")]
    public long FileSize { get; set; }
}