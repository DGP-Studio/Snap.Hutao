// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;

/// <summary>
/// 插件
/// </summary>
[HighQuality]
internal sealed class Plugin
{
    /// <summary>
    /// 插件列表
    /// </summary>
    [JsonPropertyName("plugins")]
    public List<PluginItem> Plugins { get; set; } = default!;

    /// <summary>
    /// 一般为 1
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = default!;
}
