// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher.Resource;

/// <summary>
/// 游戏资源
/// </summary>
[HighQuality]
internal sealed class GameResource
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
    /// 官网地址 https://ys.mihoyo.com/launcher
    /// </summary>
    [JsonPropertyName("web_url")]
    public Uri WebUrl { get; set; } = default!;

    /// <summary>
    /// 强制更新文件 null
    /// </summary>
    [JsonPropertyName("force_update")]
    public object? ForceUpdate { get; set; }

    /// <summary>
    /// 预下载
    /// </summary>
    [JsonPropertyName("pre_download_game")]
    public Game? PreDownloadGame { get; set; }

    /// <summary>
    /// 过期更新包
    /// </summary>
    [JsonPropertyName("deprecated_packages")]
    public List<NameMd5> DeprecatedPackages { get; set; } = default!;

    /// <summary>
    /// 渠道服 sdk
    /// </summary>
    [JsonPropertyName("sdk")]
    public Sdk? Sdk { get; set; }

    /// <summary>
    /// 过期的单个文件
    /// </summary>
    [JsonPropertyName("deprecated_files")]
    public List<NameMd5>? DeprecatedFiles { get; set; }
}