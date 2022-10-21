// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Binding.Hutao;

/// <summary>
/// 角色榜
/// </summary>
internal class ComplexAvatarRank
{
    /// <summary>
    /// 层数
    /// </summary>
    public string Floor { get; set; } = default!;

    /// <summary>
    /// 排行信息
    /// </summary>
    public List<ComplexAvatar> Avatars { get; set; } = default!;
}