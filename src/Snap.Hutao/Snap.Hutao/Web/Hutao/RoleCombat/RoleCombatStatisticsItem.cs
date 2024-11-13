// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hutao.SpiralAbyss;

namespace Snap.Hutao.Web.Hutao.RoleCombat;

internal sealed class RoleCombatStatisticsItem
{
    public int RecordTotal { get; set; }

    public List<ItemRate<AvatarId, double>> BackupAvatarRates { get; set; } = default!;
}