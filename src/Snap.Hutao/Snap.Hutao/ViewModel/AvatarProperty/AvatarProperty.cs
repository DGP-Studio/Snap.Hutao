// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Web.Endpoint.Hutao;
using System.Collections.Frozen;

namespace Snap.Hutao.ViewModel.AvatarProperty;

internal sealed partial class AvatarProperty : ObservableObject, INameIcon<Uri>
{
    private static readonly FrozenDictionary<FightProperty, Uri> PropertyIcons = WinRTAdaptive.ToFrozenDictionary(
    [
        KeyValuePair.Create(FightProperty.FIGHT_PROP_SKILL_CD_MINUS_RATIO, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_CDReduce.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_CHARGE_EFFICIENCY, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_ChargeEfficiency.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_CRITICAL, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_Critical.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_CRITICAL_HURT, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_Critical_Hurt.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_CUR_ATTACK, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_CurAttack.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_CUR_DEFENSE, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_CurDefense.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_ELEMENT_MASTERY, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_Element.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_ELEC_ADD_HURT, StaticResourcesEndpoints.StaticRaw("IconElement", "UI_Icon_Element_Electric.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_FIRE_ADD_HURT, StaticResourcesEndpoints.StaticRaw("IconElement", "UI_Icon_Element_Fire.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_GRASS_ADD_HURT, StaticResourcesEndpoints.StaticRaw("IconElement", "UI_Icon_Element_Grass.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_ICE_ADD_HURT, StaticResourcesEndpoints.StaticRaw("IconElement", "UI_Icon_Element_Ice.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_ROCK_ADD_HURT, StaticResourcesEndpoints.StaticRaw("IconElement", "UI_Icon_Element_Rock.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_WATER_ADD_HURT, StaticResourcesEndpoints.StaticRaw("IconElement", "UI_Icon_Element_Water.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_WIND_ADD_HURT, StaticResourcesEndpoints.StaticRaw("IconElement", "UI_Icon_Element_Wind.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_HEAL_ADD, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_Heal.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_MAX_HP, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_MaxHp.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_PhysicalAttackUp.png").ToUri()),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_SHIELD_COST_MINUS_RATIO, StaticResourcesEndpoints.StaticRaw("Property", "UI_Icon_ShieldCostMinus.png").ToUri()),
    ]);

    public AvatarProperty(FightProperty property, string name, string value, string? addValue = null)
    {
        FightProperty = property;
        Name = name;
        Value = value;
        Icon = PropertyIcons.GetValueOrDefault(property);
        if (!string.IsNullOrEmpty(addValue))
        {
            AddValue = addValue;
        }
    }

    public string Name { get; }

    [AllowNull]
    public Uri Icon { get; }

    public string Value { get; }

    public string? AddValue { get; }

    internal FightProperty FightProperty { get; }
}