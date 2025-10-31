// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Collections.Frozen;

namespace Snap.Hutao.Service.RoleCombat;

[Service(ServiceLifetime.Singleton, typeof(IRoleCombatRepository))]
internal sealed partial class RoleCombatRepository : IRoleCombatRepository
{
    [GeneratedConstructor]
    public partial RoleCombatRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public FrozenDictionary<uint, RoleCombatEntry> GetRoleCombatEntryMapByUid(string uid)
    {
        return this.Query(query => query
            .Where(s => s.Uid == uid)
            .OrderByDescending(s => s.ScheduleId)
            .ToFrozenDictionary(e => e.ScheduleId));
    }

    public void UpdateRoleCombatEntry(RoleCombatEntry entry)
    {
        this.Update(entry);
    }

    public void AddRoleCombatEntry(RoleCombatEntry entry)
    {
        this.Add(entry);
    }
}