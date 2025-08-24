// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Model.Intrinsic;
using System.Collections.Frozen;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

internal sealed class InGameFightPropertyComparer : IComparer<FightProperty>
{
    private static readonly FrozenDictionary<FightProperty, int> InGamePropertyOrder = WinRTAdaptive.ToFrozenDictionary(
    [
        KeyValuePair.Create(FightProperty.FIGHT_PROP_MAX_HP, 1),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_CUR_ATTACK, 2),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_CUR_DEFENSE, 3),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_ELEMENT_MASTERY, 4),
        KeyValuePair.Create(FightProperty.STAMINA, 5),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_CRITICAL, 6),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_CRITICAL_HURT, 7),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_HEAL_ADD, 8),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_HEALED_ADD, 9),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_CHARGE_EFFICIENCY, 10),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_SKILL_CD_MINUS_RATIO, 11),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_SHIELD_COST_MINUS_RATIO, 12),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_FIRE_ADD_HURT, 13),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_FIRE_SUB_HURT, 14),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_WATER_ADD_HURT, 15),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_WATER_SUB_HURT, 16),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_GRASS_ADD_HURT, 17),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_GRASS_SUB_HURT, 18),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_ELEC_ADD_HURT, 19),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_ELEC_SUB_HURT, 20),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_WIND_ADD_HURT, 21),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_WIND_SUB_HURT, 22),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_ICE_ADD_HURT, 23),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_ICE_SUB_HURT, 24),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_ROCK_ADD_HURT, 25),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_ROCK_SUB_HURT, 26),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_PHYSICAL_ADD_HURT, 27),
        KeyValuePair.Create(FightProperty.FIGHT_PROP_PHYSICAL_SUB_HURT, 28),
    ]);

    private static readonly LazySlim<InGameFightPropertyComparer> LazyShared = new(() => new());

    public static InGameFightPropertyComparer Shared { get => LazyShared.Value; }

    public int Compare(FightProperty x, FightProperty y)
    {
        return InGamePropertyOrder.GetValueOrDefault(x, (int)x).CompareTo(InGamePropertyOrder.GetValueOrDefault(y, (int)y));
    }
}