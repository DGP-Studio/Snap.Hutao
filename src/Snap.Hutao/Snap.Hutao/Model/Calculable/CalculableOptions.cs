// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Calculable;

/// <summary>
/// 可计算物品选项
/// </summary>
internal sealed class CalculableOptions
{
    /// <summary>
    /// 构造一个新的可计算物品选项
    /// </summary>
    /// <param name="avatar">角色</param>
    /// <param name="weapon">武器</param>
    public CalculableOptions(ICalculableAvatar? avatar, ICalculableWeapon? weapon)
    {
        Avatar = avatar;
        Weapon = weapon;
    }

    /// <summary>
    /// 角色
    /// </summary>
    public ICalculableAvatar? Avatar { get; }

    /// <summary>
    /// 武器
    /// </summary>
    public ICalculableWeapon? Weapon { get; }
}