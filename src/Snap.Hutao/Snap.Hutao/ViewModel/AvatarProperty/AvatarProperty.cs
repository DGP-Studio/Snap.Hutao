﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Control.Collection.Alternating;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using System.Collections.Frozen;

namespace Snap.Hutao.ViewModel.AvatarProperty;

/// <summary>
/// 角色属性值
/// </summary>
[HighQuality]
internal sealed class AvatarProperty : ObservableObject, INameIcon, IAlternatingItem
{
    private static readonly FrozenDictionary<FightProperty, Uri> PropertyIcons = new Dictionary<FightProperty, Uri>()
    {
        [FightProperty.FIGHT_PROP_SKILL_CD_MINUS_RATIO] = Web.HutaoEndpoints.StaticRaw("Property", "UI_Icon_CDReduce.png").ToUri(),
        [FightProperty.FIGHT_PROP_CHARGE_EFFICIENCY] = Web.HutaoEndpoints.StaticRaw("Property", "UI_Icon_ChargeEfficiency.png").ToUri(),
        [FightProperty.FIGHT_PROP_CRITICAL] = Web.HutaoEndpoints.StaticRaw("Property", "UI_Icon_Critical.png").ToUri(),
        [FightProperty.FIGHT_PROP_CRITICAL_HURT] = Web.HutaoEndpoints.StaticRaw("Property", "UI_Icon_Critical_Hurt.png").ToUri(),
        [FightProperty.FIGHT_PROP_CUR_ATTACK] = Web.HutaoEndpoints.StaticRaw("Property", "UI_Icon_CurAttack.png").ToUri(),
        [FightProperty.FIGHT_PROP_CUR_DEFENSE] = Web.HutaoEndpoints.StaticRaw("Property", "UI_Icon_CurDefense.png").ToUri(),
        [FightProperty.FIGHT_PROP_ELEMENT_MASTERY] = Web.HutaoEndpoints.StaticRaw("Property", "UI_Icon_Element.png").ToUri(),
        [FightProperty.FIGHT_PROP_ELEC_ADD_HURT] = Web.HutaoEndpoints.StaticRaw("Property", "UI_Icon_Element_Electric.png").ToUri(),
        [FightProperty.FIGHT_PROP_FIRE_ADD_HURT] = Web.HutaoEndpoints.StaticRaw("Property", "UI_Icon_Element_Fire.png").ToUri(),
        [FightProperty.FIGHT_PROP_GRASS_ADD_HURT] = Web.HutaoEndpoints.StaticRaw("Property", "UI_Icon_Element_Grass.png").ToUri(),
        [FightProperty.FIGHT_PROP_ICE_ADD_HURT] = Web.HutaoEndpoints.StaticRaw("Property", "UI_Icon_Element_Ice.png").ToUri(),
        [FightProperty.FIGHT_PROP_ROCK_ADD_HURT] = Web.HutaoEndpoints.StaticRaw("Property", "UI_Icon_Element_Rock.png").ToUri(),
        [FightProperty.FIGHT_PROP_WATER_ADD_HURT] = Web.HutaoEndpoints.StaticRaw("Property", "UI_Icon_Element_Water.png").ToUri(),
        [FightProperty.FIGHT_PROP_WIND_ADD_HURT] = Web.HutaoEndpoints.StaticRaw("Property", "UI_Icon_Element_Wind.png").ToUri(),
        [FightProperty.FIGHT_PROP_HEAL_ADD] = Web.HutaoEndpoints.StaticRaw("Property", "UI_Icon_Heal.png").ToUri(),
        [FightProperty.FIGHT_PROP_MAX_HP] = Web.HutaoEndpoints.StaticRaw("Property", "UI_Icon_MaxHp.png").ToUri(),
        [FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT] = Web.HutaoEndpoints.StaticRaw("Property", "UI_Icon_PhysicalAttackUp.png").ToUri(),
        [FightProperty.FIGHT_PROP_SHIELD_COST_MINUS_RATIO] = Web.HutaoEndpoints.StaticRaw("Property", "UI_Icon_ShieldCostMinus.png").ToUri(),
    }.ToFrozenDictionary();

    private Brush? background;

    /// <summary>
    /// 构造一个新的角色属性值
    /// </summary>
    /// <param name="property">战斗属性</param>
    /// <param name="name">名称</param>
    /// <param name="value">白字</param>
    /// <param name="addValue">绿字</param>
    public AvatarProperty(FightProperty property, string name, string value, string? addValue = null)
    {
        Name = name;
        Value = value;
        Icon = PropertyIcons.GetValueOrDefault(property);
        AddValue = addValue;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 图标
    /// </summary>
    [AllowNull]
    public Uri Icon { get; }

    /// <summary>
    /// 白字
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// 绿字
    /// </summary>
    public string? AddValue { get; }

    public Brush? Background { get => background; set => SetProperty(ref background, value); }
}