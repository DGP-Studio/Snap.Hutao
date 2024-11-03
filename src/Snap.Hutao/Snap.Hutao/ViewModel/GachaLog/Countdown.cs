// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Abstraction;

namespace Snap.Hutao.ViewModel.GachaLog;

internal sealed class Countdown
{

    public Countdown(ICountdownItem item, GachaEvent gachaEvent)
    {
        Item = item;
        LastTime = gachaEvent.To;

        FormattedVersionOrder = $"{gachaEvent.Version} {(gachaEvent.Order is 1 ? "上半" : "下半")}";

        int cdDays = (int)(DateTimeOffset.Now - LastTime).TotalDays;
        CountdownString = cdDays > 0 ? $"距离上次复刻已有 {cdDays} 天" : $"距离卡池结束还有 {-cdDays} 天";
    }

    public ICountdownItem Item { get; set; }

    public string FormattedLastTime
    {
        get => LastTime <= DateTimeOffset.Now ? $"上次复刻时间: {LastTime:yyyy.MM.dd}" : "当前卡池";
    }

    public string FormattedVersionOrder { get; set; }

    public string CountdownString { get; set; }

    internal DateTimeOffset LastTime { get; set; }
}