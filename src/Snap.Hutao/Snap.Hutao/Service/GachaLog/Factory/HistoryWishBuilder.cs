// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.GachaLog.Factory;

internal sealed class HistoryWishBuilder
{
    private readonly GachaEvent gachaEvent;

    private readonly Dictionary<IStatisticsItemConvertible, int> orangeUpCounter = [];
    private readonly Dictionary<IStatisticsItemConvertible, int> purpleUpCounter = [];
    private readonly Dictionary<IStatisticsItemConvertible, int> orangeCounter = [];
    private readonly Dictionary<IStatisticsItemConvertible, int> purpleCounter = [];
    private readonly Dictionary<IStatisticsItemConvertible, int> blueCounter = [];

    private int totalCountTracker;

    private HistoryWishBuilder(GachaEvent gachaEvent, GachaLogServiceMetadataContext context)
    {
        this.gachaEvent = gachaEvent;
        ConfigType = gachaEvent.Type;

        switch (ConfigType)
        {
            case GachaType.ActivityAvatar or GachaType.SpecialActivityAvatar:
                orangeUpCounter = gachaEvent.UpOrangeList.Select(id => context.IdAvatarMap[id]).ToDictionary(IStatisticsItemConvertible (a) => a, _ => 0);
                purpleUpCounter = gachaEvent.UpPurpleList.Select(id => context.IdAvatarMap[id]).ToDictionary(IStatisticsItemConvertible (a) => a, _ => 0);
                break;
            case GachaType.ActivityWeapon:
                orangeUpCounter = gachaEvent.UpOrangeList.Select(id => context.IdWeaponMap[id]).ToDictionary(IStatisticsItemConvertible (w) => w, _ => 0);
                purpleUpCounter = gachaEvent.UpPurpleList.Select(id => context.IdWeaponMap[id]).ToDictionary(IStatisticsItemConvertible (w) => w, _ => 0);
                break;
            case GachaType.ActivityCity:

                // Avatars are less than weapons, so we try to get the value from avatar map first
                orangeUpCounter = gachaEvent.UpOrangeList.Select(id => (IStatisticsItemConvertible?)context.IdAvatarMap.GetValueOrDefault(id) ?? context.IdWeaponMap[id]).ToDictionary(c => c, _ => 0);
                purpleUpCounter = gachaEvent.UpPurpleList.Select(id => (IStatisticsItemConvertible?)context.IdAvatarMap.GetValueOrDefault(id) ?? context.IdWeaponMap[id]).ToDictionary(c => c, _ => 0);
                break;
        }
    }

    public GachaType ConfigType { get; }

    public DateTimeOffset From { get => gachaEvent.From; }

    public DateTimeOffset To { get => gachaEvent.To; }

    public bool IsEmpty { get => totalCountTracker <= 0; }

    public static HistoryWishBuilder Create(GachaEvent gachaEvent, GachaLogServiceMetadataContext context)
    {
        return new(gachaEvent, context);
    }

    public bool IncreaseOrange(IStatisticsItemConvertible item)
    {
        orangeCounter.IncreaseByOne(item);
        ++totalCountTracker;

        return orangeUpCounter.TryIncreaseByOne(item);
    }

    public void IncreasePurple(IStatisticsItemConvertible item)
    {
        purpleUpCounter.TryIncreaseByOne(item);
        purpleCounter.IncreaseByOne(item);
        ++totalCountTracker;
    }

    public void IncreaseBlue(IStatisticsItemConvertible item)
    {
        blueCounter.IncreaseByOne(item);
        ++totalCountTracker;
    }

    public HistoryWish ToHistoryWish()
    {
        HistoryWish historyWish = new()
        {
            // Base
            Name = gachaEvent.Name,
            From = gachaEvent.From,
            To = gachaEvent.To,
            TotalCount = totalCountTracker,

            // Fill
            Version = gachaEvent.Version,
            BannerImage = gachaEvent.Banner,
            OrangeUpList = orangeUpCounter.ToStatisticsImmutableArray(),
            PurpleUpList = purpleUpCounter.ToStatisticsImmutableArray(),
            OrangeList = orangeCounter.ToStatisticsImmutableArray(),
            PurpleList = purpleCounter.ToStatisticsImmutableArray(),
            BlueList = blueCounter.ToStatisticsImmutableArray(),
        };

        return historyWish;
    }
}