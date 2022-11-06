// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;

/// <summary>
/// 插件项
/// </summary>
public class PluginItem : LocalFile
{
    /// <summary>
    /// 版本 一般为空
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = default!;

    /// <summary>
    /// 下载地址
    /// </summary>
    [JsonPropertyName("path")]
    public Uri Path { get; set; } = default!;

    /// <summary>
    /// 尺寸
    /// </summary>
    [JsonPropertyName("size")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long Size { get; set; } = default!;

    /// <summary>
    /// 一般为空
    /// </summary>
    [JsonPropertyName("entry")]
    public string Entry { get; set; } = default!;
}