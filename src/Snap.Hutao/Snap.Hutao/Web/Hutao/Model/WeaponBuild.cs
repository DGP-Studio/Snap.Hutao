// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hutao.Model;

/// <summary>
/// 武器相关构筑
/// </summary>
public abstract class WeaponBuild
{
    /// <summary>
    /// 角色Id
    /// </summary>
    public WeaponId WeaponId { get; set; }
}