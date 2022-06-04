// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

/// <summary>
/// 玩家信息
/// </summary>
public class PlayerInfo
{
    /// <summary>
    /// 持有的角色的信息
    /// </summary>
    [JsonPropertyName("avatars")]
    public List<Avatar.Avatar> Avatars { get; set; } = default!;

    /// <summary>
    /// 玩家的基本信息
    /// </summary>
    [JsonPropertyName("stats")]
    public PlayerStats PlayerStat { get; set; } = default!;

    /// <summary>
    /// 世界探索
    /// </summary>
    [JsonPropertyName("world_explorations")]
    public List<WorldExploration> WorldExplorations { get; set; } = default!;

    /// <summary>
    /// 洞天
    /// </summary>
    [JsonPropertyName("homes")]
    public List<Home> Homes { get; set; } = default!;
}
