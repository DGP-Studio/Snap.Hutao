// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher.Resource;

/// <summary>
/// 语音包
/// </summary>
[HighQuality]
internal sealed class VoicePackage : PathMd5
{
    /// <summary>
    /// 语音
    /// </summary>
    [JsonPropertyName("language")]
    public string Language { get; set; } = default!;

    /// <summary>
    /// 名称 一般为空
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 解压尺寸
    /// </summary>
    [JsonPropertyName("size")]
    public long Size { get; set; }

    /// <summary>
    /// 包尺寸
    /// </summary>
    [JsonPropertyName("package_size")]
    public long PackageSize { get; set; } = default!;
}
