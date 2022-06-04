// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Snap.Hutao.Web.Hutao.Model;

/// <summary>
/// 武器使用数据
/// </summary>
public class AvatarWeaponUsage
{
    /// <summary>
    /// 角色Id
    /// </summary>
    public int Avatar { get; set; }

    /// <summary>
    /// 武器比率
    /// </summary>
    public IEnumerable<Rate<int>> Weapons { get; set; } = default!;
}