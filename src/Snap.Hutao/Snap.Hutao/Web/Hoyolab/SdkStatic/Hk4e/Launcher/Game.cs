// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;

/// <summary>
/// 游戏
/// </summary>
[HighQuality]
internal sealed class Game
{
    /// <summary>
    /// 最新客户端
    /// </summary>
    [JsonPropertyName("latest")]
    public LatestPackage Latest { get; set; } = default!;

    /// <summary>
    /// 相对于当前版本的之前版本的差异文件（非预下载）
    /// </summary>
    [JsonPropertyName("diffs")]
    public List<DiffPackage> Diffs { get; set; } = default!;
}
