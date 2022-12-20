// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

namespace Snap.Hutao.Model.Calculable;

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
    public CalculableWeapon(Metadata.Weapon.Weapon weapon)
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

    /// <summary>
    /// 构造一个新的可计算武器
    /// </summary>
    /// <param name="weapon">武器</param>
    public CalculableWeapon(Binding.AvatarProperty.Weapon weapon)
    {
        WeaponId = weapon.Id;
        LevelMin = weapon.LevelNumber;
        LevelMax = (int)weapon.Quality >= 3 ? 90 : 70;
        Name = weapon.Name;
        Icon = weapon.Icon;
        Quality = weapon.Quality;

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