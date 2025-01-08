// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Collections.Frozen;

namespace Snap.Hutao.Service.SpiralAbyss;

internal interface ISpiralAbyssRecordRepository : IRepository<SpiralAbyssEntry>
{
    void AddSpiralAbyssEntry(SpiralAbyssEntry entry);

    FrozenDictionary<uint, SpiralAbyssEntry> GetSpiralAbyssEntryMapByUid(string uid);

    void UpdateSpiralAbyssEntry(SpiralAbyssEntry entry);
}