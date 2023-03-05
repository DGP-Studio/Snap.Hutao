// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.Complex;

/// <summary>
/// 角色榜
/// </summary>
[HighQuality]
internal sealed class AvatarRankView
{
    /// <summary>
    /// 层数
    /// </summary>
    public string Floor { get; set; } = default!;

    /// <summary>
    /// 排行信息
    /// </summary>
    public List<AvatarView> Avatars { get; set; } = default!;
}