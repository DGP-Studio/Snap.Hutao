// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;

/// <summary>
/// 资源文件
/// </summary>
[HighQuality]
internal class NameMd5
{
    /// <summary>
    /// 文件名称
    /// 相对于 EXE 的路径，如 YuanShen_Data/Plugins/PCGameSDK.dll
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// Md5 校验值
    /// </summary>
    [JsonPropertyName("md5")]
    public string Md5 { get; set; } = default!;
}
