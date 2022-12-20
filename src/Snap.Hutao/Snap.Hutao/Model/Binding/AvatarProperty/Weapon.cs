// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

namespace Snap.Hutao.Model.Binding.AvatarProperty;

/// <summary>
/// 武器
/// </summary>
public class Weapon : EquipBase, ICalculableSource<ICalculableWeapon>
{
    /// <summary>
    /// 副属性
    /// </summary>
    public Pair<string, string> SubProperty { get; set; } = default!;

    /// <summary>
    /// 精炼属性
    /// </summary>
    public string AffixLevel { get; set; } = default!;

    /// <summary>
    /// 精炼名称
    /// </summary>
    public string AffixName { get; set; } = default!;

    /// <summary>
    /// 精炼被动
    /// </summary>
    public string AffixDescription { get; set; } = default!;

    /// <summary>
    /// Id
    /// </summary>
    internal WeaponId Id { get; set; }

    /// <summary>
    /// 等级数字
    /// </summary>
    internal int LevelNumber { get; set; }

    /// <inheritdoc/>
    public ICalculableWeapon ToCalculable()
    {
        return new CalculableWeapon(this);
    }
}