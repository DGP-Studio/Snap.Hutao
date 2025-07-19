// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.RoleCombat;

internal sealed class SplendourBuffView
{
    private SplendourBuffView(RoleCombatSplendourBuff roleCombatSplendourBuff)
    {
        Icon = roleCombatSplendourBuff.Icon.ToUri();
        Level = roleCombatSplendourBuff.Level;
        Name = roleCombatSplendourBuff.Name;

        // TODO: It's a miHoYo description syntax string
        Effects = roleCombatSplendourBuff.LevelEffects.SelectAsArray(static e => e.Description.Replace("\\n", "\n", StringComparison.OrdinalIgnoreCase));
    }

    public Uri Icon { get; }

    public int Level { get; }

    public string Name { get; }

    public ImmutableArray<string> Effects { get; }

    public static SplendourBuffView Create(RoleCombatSplendourBuff roleCombatSplendourBuff)
    {
        return new(roleCombatSplendourBuff);
    }
}