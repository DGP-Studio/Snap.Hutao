// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.SpiralAbyss;

internal interface ISpiralAbyssRecordRepository : IRepository<SpiralAbyssEntry>
{
    void AddSpiralAbyssEntry(SpiralAbyssEntry entry);

    Dictionary<uint, SpiralAbyssEntry> GetSpiralAbyssEntryMapByUid(string uid);

    void UpdateSpiralAbyssEntry(SpiralAbyssEntry entry);
}