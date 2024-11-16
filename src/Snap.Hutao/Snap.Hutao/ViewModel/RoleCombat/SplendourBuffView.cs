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

    public Uri Icon { get; set; }

    public int Level { get; set; }

    public string Name { get; set; }

    public List<string> Effects { get; set; }

    public static SplendourBuffView From(RoleCombatSplendourBuff roleCombatSplendourBuff)
    {
        return new(roleCombatSplendourBuff);
    }
}
