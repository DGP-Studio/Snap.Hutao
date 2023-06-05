// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Calculable;

/// <summary>
/// 可计算的武器
/// </summary>
[HighQuality]
internal interface ICalculableWeapon : ICalculable
{
    /// <summary>
    /// 武器Id
    /// </summary>
    WeaponId WeaponId { get; }

    /// <summary>
    /// 最小等级
    /// </summary>
    uint LevelMin { get; }

    /// <summary>
    /// 最大等级
    /// </summary>
    uint LevelMax { get; }
}