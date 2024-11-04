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

        FormattedVersionOrder = $"{gachaEvent.Version} {(gachaEvent.Order is 1 ? SH.ViewModelGachaLogCountdownOrderUp : SH.ViewModelGachaLogCountdownOrderDown)}";

        int cdDays = (int)(DateTimeOffset.Now - LastTime).TotalDays;
        FormattedCountdown = cdDays > 0 ? SH.FormatViewModelGachaLogCountdownLastTimeDelta(cdDays) : SH.FormatViewModelGachaLogCountdownCurrentWishDelta(-cdDays);
    }

    public string FormattedLastTime
    {
        get => LastTime <= DateTimeOffset.Now ? SH.FormatViewModelGachaLogCountdownLastTime(LastTime) : SH.ViewModelGachaLogCountdownCurrentWish;
    }

    public string FormattedVersionOrder { get; set; }

    public string FormattedCountdown { get; set; }

    public ICountdownItem Item { get; set; }

    internal DateTimeOffset LastTime { get; set; }
}