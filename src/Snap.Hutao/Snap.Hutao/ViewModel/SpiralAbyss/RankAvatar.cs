// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.SpiralAbyss;

/// <summary>
/// 排行角色
/// </summary>
[HighQuality]
internal sealed class RankAvatar : AvatarView
{
    public RankAvatar(int value, Model.Metadata.Avatar.Avatar avatar)
        : base(avatar)
    {
        Value = value;
    }

    /// <summary>
    /// 排行
    /// </summary>
    public int Value { get; }
}