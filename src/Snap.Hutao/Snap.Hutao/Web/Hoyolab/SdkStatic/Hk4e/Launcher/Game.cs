// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;

/// <summary>
/// 游戏
/// </summary>
public class Game
{
    /// <summary>
    /// 最新客户端
    /// </summary>
    [JsonPropertyName("latest")]
    public Package Latest { get; set; } = default!;

    /// <summary>
    /// 差异文件
    /// </summary>
    [JsonPropertyName("diffs")]
    public IList<DiffPackage> Diffs { get; set; } = default!;
}
