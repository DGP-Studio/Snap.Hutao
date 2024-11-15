﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.ViewModel.GachaLog;

namespace Snap.Hutao.Service.GachaLog.Factory;

internal interface IGachaStatisticsFactory
{
    ValueTask<GachaStatistics> CreateAsync(List<GachaItem> items, GachaLogServiceMetadataContext context);
}