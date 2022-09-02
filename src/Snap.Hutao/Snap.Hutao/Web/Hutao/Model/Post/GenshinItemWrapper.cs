// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Model.Post;

/// <summary>
/// 原神物品包装器
/// </summary>
public class GenshinItemWrapper
{
    /// <summary>
    /// 构造一个新的原神物品包装器
    /// </summary>
    /// <param name="avatars">角色</param>
    /// <param name="weapons">武器</param>
    /// <param name="reliquaries">圣遗物</param>
    public GenshinItemWrapper(IEnumerable<Item> avatars, IEnumerable<Item> weapons, IEnumerable<Item> reliquaries)
    {
        Avatars = avatars;
        Weapons = weapons;
        Reliquaries = reliquaries;
    }

    /// <summary>
    /// 角色列表
    /// </summary>
    public IEnumerable<Item> Avatars { get; }

    /// <summary>
    /// 武器列表
    /// </summary>
    public IEnumerable<Item> Weapons { get; }

    /// <summary>
    /// 圣遗物列表
    /// </summary>
    public IEnumerable<Item> Reliquaries { get; }
}
