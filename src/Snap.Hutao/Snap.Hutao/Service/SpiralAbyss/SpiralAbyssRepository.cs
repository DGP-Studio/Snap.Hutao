// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Collections.Frozen;

namespace Snap.Hutao.Service.SpiralAbyss;

[Service(ServiceLifetime.Singleton, typeof(ISpiralAbyssRepository))]
internal sealed partial class SpiralAbyssRepository : ISpiralAbyssRepository
{
    [GeneratedConstructor]
    public partial SpiralAbyssRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public FrozenDictionary<uint, SpiralAbyssEntry> GetSpiralAbyssEntryMapByUid(string uid)
    {
        return this.Query(query => query
            .Where(s => s.Uid == uid)
            .OrderByDescending(s => s.ScheduleId)
            .ToFrozenDictionary(e => e.ScheduleId));
    }

    public void UpdateSpiralAbyssEntry(SpiralAbyssEntry entry)
    {
        this.Update(entry);
    }

    public void AddSpiralAbyssEntry(SpiralAbyssEntry entry)
    {
        this.Add(entry);
    }
}