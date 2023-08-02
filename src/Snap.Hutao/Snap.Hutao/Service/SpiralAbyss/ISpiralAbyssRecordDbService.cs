// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.SpiralAbyss;

internal interface ISpiralAbyssRecordDbService
{
    ValueTask AddSpiralAbyssEntryAsync(SpiralAbyssEntry entry);

    ValueTask<ObservableCollection<SpiralAbyssEntry>> GetSpiralAbyssEntryCollectionByUidAsync(string uid);

    ValueTask UpdateSpiralAbyssEntryAsync(SpiralAbyssEntry entry);
}