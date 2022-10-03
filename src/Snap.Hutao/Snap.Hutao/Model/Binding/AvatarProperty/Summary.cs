// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Binding.AvatarProperty;

/// <summary>
/// 玩家与角色列表的包装器
/// </summary>
public class Summary
{
    /// <summary>
    /// 玩家信息
    /// </summary>
    public Player Player { get; set; } = default!;

    /// <summary>
    /// 角色列表
    /// </summary>
    public List<Avatar> Avatars { get; set; } = default!;
}