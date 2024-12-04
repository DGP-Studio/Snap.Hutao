// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.ViewModel.GachaLog;

namespace Snap.Hutao.Model.Metadata.Abstraction;

internal interface ISummaryItemConvertible
{
    QualityType Quality { get; }

    SummaryItem ToSummaryItem(int lastPull, in DateTimeOffset time, bool isUp);
}