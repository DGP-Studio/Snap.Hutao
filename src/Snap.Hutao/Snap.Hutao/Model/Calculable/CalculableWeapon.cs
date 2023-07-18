// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.Model.Calculable;

/// <summary>
/// 可计算武器
/// </summary>
[HighQuality]
internal sealed class CalculableWeapon
    : ObservableObject,
    ICalculableWeapon,
    IMappingFrom<CalculableWeapon, Weapon>,
    IMappingFrom<CalculableWeapon, WeaponView>
{
    private uint levelCurrent;
    private uint levelTarget;

    /// <summary>
    /// 构造一个新的可计算武器
    /// </summary>
    /// <param name="weapon">武器</param>
    private CalculableWeapon(Weapon weapon)
    {
        WeaponId = weapon.Id;
        LevelMin = 1;
        LevelMax = weapon.MaxLevel;
        Name = weapon.Name;
        Icon = EquipIconConverter.IconNameToUri(weapon.Icon);
        Quality = weapon.RankLevel;

        LevelCurrent = LevelMin;
        LevelTarget = LevelMax;
    }

    /// <summary>
    /// 构造一个新的可计算武器
    /// </summary>
    /// <param name="weapon">武器</param>
    private CalculableWeapon(WeaponView weapon)
    {
        WeaponId = weapon.Id;
        LevelMin = weapon.LevelNumber;
        LevelMax = weapon.MaxLevel;
        Name = weapon.Name;
        Icon = weapon.Icon;
        Quality = weapon.Quality;

        LevelCurrent = LevelMin;
        LevelTarget = LevelMax;
    }

    /// <inheritdoc/>
    public WeaponId WeaponId { get; }

    /// <inheritdoc/>
    public uint LevelMin { get; }

    /// <inheritdoc/>
    public uint LevelMax { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public Uri Icon { get; }

    /// <inheritdoc/>
    public QualityType Quality { get; }

    /// <inheritdoc/>
    public uint LevelCurrent { get => levelCurrent; set => SetProperty(ref levelCurrent, value); }

    /// <inheritdoc/>
    public uint LevelTarget { get => levelTarget; set => SetProperty(ref levelTarget, value); }

    public static CalculableWeapon From(Weapon source)
    {
        return new(source);
    }

    public static CalculableWeapon From(WeaponView source)
    {
        return new(source);
    }
}