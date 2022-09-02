// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

/// <summary>
/// 等级经验
/// </summary>
public class LevelExperience
{
    /// <summary>
    /// 等级
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; }

    /// <summary>
    /// 经验
    /// </summary>
    [JsonPropertyName("exp")]
    public int Exp { get; set; }

    /// <summary>
    /// 游戏Id
    /// 1 大别野
    /// 2 原神
    /// 3 崩坏学园2
    /// 4 未定事件簿
    /// 5 崩坏：星穹铁道
    /// 6 绝区零
    /// 8 崩坏3
    /// </summary>
    [JsonPropertyName("game_id")]
    public int GameId { get; set; }
}
