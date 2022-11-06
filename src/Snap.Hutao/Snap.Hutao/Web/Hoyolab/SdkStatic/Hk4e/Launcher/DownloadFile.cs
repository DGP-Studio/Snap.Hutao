// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;

/// <summary>
/// 下载的文件
/// </summary>
public class DownloadFile
{
    /// <summary>
    /// 下载地址
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; } = default!;

    /// <summary>
    /// MD5校验
    /// </summary>
    [JsonPropertyName("md5")]
    public string Md5 { get; set; } = default!;
}
