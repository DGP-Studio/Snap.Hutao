// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model.Binding.Gacha;
using Snap.Hutao.Model.Binding.Gacha.Abstraction;
using Snap.Hutao.Model.Binding.Hutao;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

namespace Snap.Hutao.Model.Metadata.Weapon;

/// <summary>
/// 可计算武器
/// </summary>
public class CalculableWeapon : ObservableObject, ICalculableWeapon
{
    private int levelCurrent;
    private int levelTarget;

    /// <summary>
    /// 构造一个新的可计算武器
    /// </summary>
    /// <param name="weapon">武器</param>
    public CalculableWeapon(Weapon weapon)
    {
        WeaponId = weapon.Id;
        LevelMin = 1;
        LevelMax = int.Parse(weapon.Property.Parameters.Last().Level);
        Name = weapon.Name;
        Icon = EquipIconConverter.IconNameToUri(weapon.Icon);
        Quality = weapon.RankLevel;
        LevelCurrent = LevelMin;
        LevelTarget = LevelMax;
    }

    /// <inheritdoc/>
    public WeaponId WeaponId { get; }

    /// <inheritdoc/>
    public int LevelMin { get; }

    /// <inheritdoc/>
    public int LevelMax { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public Uri Icon { get; }

    /// <inheritdoc/>
    public ItemQuality Quality { get; }

    /// <inheritdoc/>
    public int LevelCurrent { get => levelCurrent; set => SetProperty(ref levelCurrent, value); }

    /// <inheritdoc/>
    public int LevelTarget { get => levelTarget; set => SetProperty(ref levelTarget, value); }
}