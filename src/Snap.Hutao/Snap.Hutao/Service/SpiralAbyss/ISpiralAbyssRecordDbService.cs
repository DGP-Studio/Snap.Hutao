// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Service.SpiralAbyss;

internal interface ISpiralAbyssRecordDbService
{
    ValueTask AddSpiralAbyssEntryAsync(SpiralAbyssEntry entry);

    ValueTask<Dictionary<uint, SpiralAbyssEntry>> GetSpiralAbyssEntryListByUidAsync(string uid);

    ValueTask UpdateSpiralAbyssEntryAsync(SpiralAbyssEntry entry);
}