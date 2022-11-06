// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;

/// <summary>
/// 游戏资源
/// </summary>
public class GameResource
{
    /// <summary>
    /// 本体
    /// </summary>
    [JsonPropertyName("game")]
    public Game Game { get; set; } = default!;

    /// <summary>
    /// 插件
    /// </summary>
    [JsonPropertyName("plugin")]
    public Plugin Plugin { get; set; } = default!;

    /// <summary>
    /// 官网地址
    /// </summary>
    [JsonPropertyName("web_url")]
    public Uri WebUrl { get; set; } = default!;

    /// <summary>
    /// 强制更新文件
    /// </summary>
    [JsonPropertyName("force_update")]
    public object? ForceUpdate { get; set; }

    /// <summary>
    /// 预下载
    /// </summary>
    [JsonPropertyName("pre_download_game")]
    public object? PreDownloadGame { get; set; }

    /// <summary>
    /// 过期更新包
    /// </summary>
    [JsonPropertyName("deprecated_packages")]
    public List<LocalFile> DeprecatedPackages { get; set; } = default!;

    /// <summary>
    /// 过期的单个文件
    /// </summary>
    [JsonPropertyName("deprecated_files")]
    public List<LocalFile>? DeprecatedFiles { get; set; }
}