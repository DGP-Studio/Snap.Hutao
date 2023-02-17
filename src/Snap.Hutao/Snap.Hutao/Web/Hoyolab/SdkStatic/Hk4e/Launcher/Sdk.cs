// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;

/// <summary>
/// Sdk
/// </summary>
[HighQuality]
internal sealed class Sdk : PathMd5
{
    /// <summary>
    /// 版本
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = default!;

    /// <summary>
    /// 常量 sdk_pkg_version
    /// </summary>
    [JsonPropertyName("pkg_version")]
    public string PackageVersion { get; set; } = default!;

    /// <summary>
    /// 描述
    /// </summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; } = default!;
}