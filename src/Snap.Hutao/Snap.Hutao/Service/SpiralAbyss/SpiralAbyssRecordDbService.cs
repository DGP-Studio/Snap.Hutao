// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.SpiralAbyss;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(ISpiralAbyssRecordDbService))]
internal sealed partial class SpiralAbyssRecordDbService : ISpiralAbyssRecordDbService
{
    private readonly IServiceProvider serviceProvider;

    public IServiceProvider ServiceProvider { get => serviceProvider; }

    public Dictionary<uint, SpiralAbyssEntry> GetSpiralAbyssEntryMapByUid(string uid)
    {
        return this.Query(query => query
            .Where(s => s.Uid == uid)
            .OrderByDescending(s => s.ScheduleId)
            .ToDictionary(e => e.ScheduleId));
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