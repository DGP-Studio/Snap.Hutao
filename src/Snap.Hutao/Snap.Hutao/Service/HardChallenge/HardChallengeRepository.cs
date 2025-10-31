// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Collections.Frozen;

namespace Snap.Hutao.Service.HardChallenge;

[Service(ServiceLifetime.Singleton, typeof(IHardChallengeRepository))]
internal sealed partial class HardChallengeRepository : IHardChallengeRepository
{
    [GeneratedConstructor]
    public partial HardChallengeRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public FrozenDictionary<uint, HardChallengeEntry> GetHardChallengeMapByUid(string uid)
    {
        return this.Query(query => query
            .Where(s => s.Uid == uid)
            .OrderByDescending(s => s.ScheduleId)
            .ToFrozenDictionary(e => e.ScheduleId));
    }

    public void UpdateHardChallengeEntry(HardChallengeEntry entry)
    {
        this.Update(entry);
    }

    public void AddHardChallengeEntry(HardChallengeEntry entry)
    {
        this.Add(entry);
    }
}