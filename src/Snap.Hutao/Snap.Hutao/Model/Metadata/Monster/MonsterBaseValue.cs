// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Converter;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Monster;

internal sealed class MonsterBaseValue : BaseValue
{
    public required float FireSubHurt { get; init; }

    public required float GrassSubHurt { get; init; }

    public required float WaterSubHurt { get; init; }

    public required float ElecSubHurt { get; init; }

    public required float WindSubHurt { get; init; }

    public required float IceSubHurt { get; init; }

    public required float RockSubHurt { get; init; }

    public required float PhysicalSubHurt { get; init; }

    public ImmutableArray<NameStringValue> SubHurts
    {
        get
        {
            return !field.IsDefault ? field : field =
            [
                FightPropertyFormat.ToNameStringValue(FightProperty.FIGHT_PROP_FIRE_SUB_HURT, FireSubHurt),
                FightPropertyFormat.ToNameStringValue(FightProperty.FIGHT_PROP_WATER_SUB_HURT, WaterSubHurt),
                FightPropertyFormat.ToNameStringValue(FightProperty.FIGHT_PROP_GRASS_SUB_HURT, GrassSubHurt),
                FightPropertyFormat.ToNameStringValue(FightProperty.FIGHT_PROP_ELEC_SUB_HURT, ElecSubHurt),
                FightPropertyFormat.ToNameStringValue(FightProperty.FIGHT_PROP_WIND_SUB_HURT, WindSubHurt),
                FightPropertyFormat.ToNameStringValue(FightProperty.FIGHT_PROP_ICE_SUB_HURT, IceSubHurt),
                FightPropertyFormat.ToNameStringValue(FightProperty.FIGHT_PROP_ROCK_SUB_HURT, RockSubHurt),
                FightPropertyFormat.ToNameStringValue(FightProperty.FIGHT_PROP_PHYSICAL_SUB_HURT, PhysicalSubHurt),
            ];
        }
    }
}