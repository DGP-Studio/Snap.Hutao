// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Collections.Frozen;

namespace Snap.Hutao.Service.RoleCombat;

internal interface IRoleCombatRepository : IRepository<RoleCombatEntry>
{
    void AddRoleCombatEntry(RoleCombatEntry entry);

    FrozenDictionary<uint, RoleCombatEntry> GetRoleCombatEntryMapByUid(string uid);

    void UpdateRoleCombatEntry(RoleCombatEntry entry);
}