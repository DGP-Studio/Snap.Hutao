// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;

/// <summary>
/// 最新客户端
/// </summary>
public class Package : PathMd5
{
    /// <summary>
    /// 名称 空
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 版本
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = default!;

    /// <summary>
    /// 尺寸
    /// </summary>
    [JsonPropertyName("size")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long Size { get; set; } = default!;

    /// <summary>
    /// 主程序相对路径 YuanShen.exe
    /// </summary>
    public string Entry { get; set; } = default!;

    /// <summary>
    /// 语音包
    /// </summary>
    [JsonPropertyName("voice_packs")]
    public List<VoicePackage> VoicePacks { get; set; } = default!;

    // We don't want to support
    // decompressed_path & segments

    /// <summary>
    /// 包大小 bytes
    /// </summary>
    [JsonPropertyName("package_size")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long PackageSize { get; set; } = default!;
}
