// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

namespace Snap.Hutao.ViewModel.RoleCombat;

internal sealed class SplendourBuffView
{
    private SplendourBuffView(RoleCombatSplendourBuff roleCombatSplendourBuff)
    {
        Icon = roleCombatSplendourBuff.Icon.ToUri();
        Level = roleCombatSplendourBuff.Level;
        Name = roleCombatSplendourBuff.Name;
        Effects = roleCombatSplendourBuff.LevelEffects.Select(e => e.Description.Replace("\\n", "\n", StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public Uri Icon { get; }

    public int Level { get; }

    public string Name { get; }

    public List<string> Effects { get; }

    public static SplendourBuffView From(RoleCombatSplendourBuff roleCombatSplendourBuff)
    {
        return new(roleCombatSplendourBuff);
    }
}
