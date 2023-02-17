// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

/// <summary>
/// 包装详细角色信息列表
/// </summary>
[HighQuality]
internal sealed class CharacterWrapper
{
    /// <summary>
    /// 角色列表
    /// </summary>
    [JsonPropertyName("avatars")]
    public List<Character> Avatars { get; set; } = default!;

    /// <summary>
    /// 玩家角色信息
    /// </summary>
    [JsonPropertyName("role")]
    public Role Role { get; set; } = default!;
}