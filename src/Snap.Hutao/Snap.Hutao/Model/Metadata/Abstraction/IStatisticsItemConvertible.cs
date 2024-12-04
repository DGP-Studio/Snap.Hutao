// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.GachaLog;

namespace Snap.Hutao.Model.Metadata.Abstraction;

internal interface IStatisticsItemConvertible
{
    StatisticsItem ToStatisticsItem(int count);
}