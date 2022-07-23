// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Model.Post;

/// <summary>
/// 伤害信息
/// </summary>
public class Damage
{
    /// <summary>
    /// 构造一个新的伤害信息
    /// </summary>
    /// <param name="avatarId">角色Id</param>
    /// <param name="value">值</param>
    public Damage(int avatarId, int value)
    {
        AvatarId = avatarId;
        Value = value;
    }

    /// <summary>
    /// 角色Id
    /// </summary>
    public int AvatarId { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public int Value { get; set; }
}