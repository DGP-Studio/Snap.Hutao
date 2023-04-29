// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.AvatarProperty;

/// <summary>
/// 玩家与角色列表的包装器
/// </summary>
[HighQuality]
internal sealed class Summary
{
    /// <summary>
    /// 角色列表
    /// </summary>
    public List<AvatarView> Avatars { get; set; } = default!;

    /// <summary>
    /// 服务器消息
    /// </summary>
    public string Message { get; set; } = default!;
}