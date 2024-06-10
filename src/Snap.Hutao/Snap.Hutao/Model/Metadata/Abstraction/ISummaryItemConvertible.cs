// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.ViewModel.GachaLog;

namespace Snap.Hutao.Model.Metadata.Abstraction;

/// <summary>
/// 指示该类为简述统计物品的源
/// </summary>
[HighQuality]
internal interface ISummaryItemConvertible
{
    QualityType Quality { get; }

    SummaryItem ToSummaryItem(int lastPull, in DateTimeOffset time, bool isUp);
}