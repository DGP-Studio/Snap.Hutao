// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Snap.Hutao.Web.Hutao.Model.Post;

/// <summary>
/// 角色武器
/// </summary>
public class AvatarWeapon
{
    /// <summary>
    /// 构造一个新的角色武器
    /// </summary>
    /// <param name="id">武器Id</param>
    /// <param name="level">武器等级</param>
    /// <param name="affixLevel">精炼等级</param>
    public AvatarWeapon(int id, int level, int affixLevel)
    {
        Id = id;
        Level = level;
        AffixLevel = affixLevel;
    }

    /// <summary>
    /// 武器等级
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// 武器等级
    /// </summary>
    public int Level { get; }

    /// <summary>
    /// 精炼
    /// </summary>
    public int AffixLevel { get; }
}
