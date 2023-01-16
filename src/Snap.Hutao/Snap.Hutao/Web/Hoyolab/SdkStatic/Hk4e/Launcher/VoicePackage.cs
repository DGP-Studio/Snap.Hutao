// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;

/// <summary>
/// 语音包
/// </summary>
public class VoicePackage : PathMd5
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
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long Size { get; set; }

    /// <summary>
    /// 包尺寸
    /// </summary>
    [JsonPropertyName("package_size")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long PackageSize { get; set; } = default!;
}
