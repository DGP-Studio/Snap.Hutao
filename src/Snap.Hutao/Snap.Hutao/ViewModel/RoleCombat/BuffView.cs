// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

namespace Snap.Hutao.ViewModel.RoleCombat;

internal sealed class BuffView
{
    private BuffView(RoleCombatBuff roleCombatBuff)
    {
        Icon = roleCombatBuff.Icon.ToUri();
        Name = roleCombatBuff.Name;
        Description = roleCombatBuff.Description.Replace("\\n", "\n", StringComparison.OrdinalIgnoreCase);
    }

    public Uri Icon { get; }

    public string Name { get; }

    public string Description { get; }

    public static BuffView Create(RoleCombatBuff roleCombatBuff)
    {
        return new(roleCombatBuff);
    }
}