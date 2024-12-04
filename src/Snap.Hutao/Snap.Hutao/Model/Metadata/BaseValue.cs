// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata;

internal class BaseValue
{
    public required float HpBase { get; init; }

    public required float AttackBase { get; init; }

    public required float DefenseBase { get; init; }

    public virtual float GetValue(FightProperty fightProperty)
    {
        return fightProperty switch
        {
            FightProperty.FIGHT_PROP_BASE_HP => HpBase,
            FightProperty.FIGHT_PROP_BASE_ATTACK => AttackBase,
            FightProperty.FIGHT_PROP_BASE_DEFENSE => DefenseBase,
            _ => 0,
        };
    }
}