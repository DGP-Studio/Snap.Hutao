// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.RoleCombat;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IRoleCombatRepository))]
internal sealed partial class RoleCombatRepository : IRoleCombatRepository
{
    public partial IServiceProvider ServiceProvider { get; }

    public Dictionary<uint, RoleCombatEntry> GetRoleCombatEntryMapByUid(string uid)
    {
        return this.Query(query => query
            .Where(s => s.Uid == uid)
            .OrderByDescending(s => s.ScheduleId)
            .ToDictionary(e => e.ScheduleId));
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