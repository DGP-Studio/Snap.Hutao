// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Binding.SpiralAbyss;

/// <summary>
/// 排行角色
/// </summary>
public class RankAvatar : Avatar
{
    /// <summary>
    /// 构造一个新的角色
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="avatarId">角色Id</param>
    /// <param name="idAvatarMap">Id角色映射</param>
    public RankAvatar(int value, AvatarId avatarId, Dictionary<AvatarId, Metadata.Avatar.Avatar> idAvatarMap)
        : base(avatarId, idAvatarMap)
    {
        Value = value;
    }

    public int Value { get; set; }
}