// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;

namespace Snap.Hutao.ViewModel.GachaLog;

internal sealed class Countdown
{
    public Countdown(Item item)
    {
        Item = item;
    }

    public string FormattedLastTime
    {
        get => LastTime <= DateTimeOffset.Now ? SH.FormatViewModelGachaLogCountdownLastTime(LastTime) : SH.ViewModelGachaLogCountdownCurrentWish;
    }

    public string FormattedVersionOrder { get => Histories.First().FormattedVersionOrder; }

    public string FormattedCountdown
    {
        get
        {
            int cdDays = (int)(DateTimeOffset.Now - LastTime).TotalDays;
            return cdDays > 0 ? SH.FormatViewModelGachaLogCountdownLastTimeDelta(cdDays) : SH.FormatViewModelGachaLogCountdownCurrentWishDelta(-cdDays);
        }
    }

    public string FormattedHistoryCount { get => SH.FormatViewModelGachaLogCountdownHistoryCount(Histories.Count); }

    public Item Item { get; }

    public List<CountdownHistory> Histories { get; } = [];

    internal DateTimeOffset LastTime { get => Histories.FirstOrDefault()?.LastTime ?? default; }
}