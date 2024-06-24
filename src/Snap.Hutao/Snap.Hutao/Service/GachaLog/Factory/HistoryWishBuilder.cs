// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.GachaLog.Factory;

/// <summary>
/// 卡池历史记录建造器
/// </summary>
[HighQuality]
internal sealed class HistoryWishBuilder
{
    private readonly GachaEvent gachaEvent;

    private readonly Dictionary<IStatisticsItemConvertible, int> orangeUpCounter = [];
    private readonly Dictionary<IStatisticsItemConvertible, int> purpleUpCounter = [];
    private readonly Dictionary<IStatisticsItemConvertible, int> orangeCounter = [];
    private readonly Dictionary<IStatisticsItemConvertible, int> purpleCounter = [];
    private readonly Dictionary<IStatisticsItemConvertible, int> blueCounter = [];

    private int totalCountTracker;

    /// <summary>
    /// 构造一个新的卡池历史记录建造器
    /// </summary>
    /// <param name="gachaEvent">卡池配置</param>
    /// <param name="context">祈愿记录上下文</param>
    public HistoryWishBuilder(GachaEvent gachaEvent, GachaLogServiceMetadataContext context)
    {
        this.gachaEvent = gachaEvent;
        ConfigType = gachaEvent.Type;

        switch (ConfigType)
        {
            case GachaType.ActivityAvatar or GachaType.SpecialActivityAvatar:
                orangeUpCounter = gachaEvent.UpOrangeList.Select(id => context.IdAvatarMap[id]).ToDictionary(a => (IStatisticsItemConvertible)a, a => 0);
                purpleUpCounter = gachaEvent.UpPurpleList.Select(id => context.IdAvatarMap[id]).ToDictionary(a => (IStatisticsItemConvertible)a, a => 0);
                break;
            case GachaType.ActivityWeapon:
                orangeUpCounter = gachaEvent.UpOrangeList.Select(id => context.IdWeaponMap[id]).ToDictionary(w => (IStatisticsItemConvertible)w, w => 0);
                purpleUpCounter = gachaEvent.UpPurpleList.Select(id => context.IdWeaponMap[id]).ToDictionary(w => (IStatisticsItemConvertible)w, w => 0);
                break;
            case GachaType.ActivityCity:

                // Avatars are less than weapons, so we try to get the value from avatar map first
                orangeUpCounter = gachaEvent.UpOrangeList.Select(id => (IStatisticsItemConvertible?)context.IdAvatarMap.GetValueOrDefault(id) ?? context.IdWeaponMap[id]).ToDictionary(c => c, c => 0);
                purpleUpCounter = gachaEvent.UpPurpleList.Select(id => (IStatisticsItemConvertible?)context.IdAvatarMap.GetValueOrDefault(id) ?? context.IdWeaponMap[id]).ToDictionary(c => c, c => 0);
                break;
        }
    }

    /// <summary>
    /// 祈愿配置类型
    /// </summary>
    public GachaType ConfigType { get; }

    /// <inheritdoc cref="GachaEvent.From"/>
    public DateTimeOffset From { get => gachaEvent.From; }

    /// <inheritdoc cref="GachaEvent.To"/>
    public DateTimeOffset To { get => gachaEvent.To; }

    /// <summary>
    /// 卡池是否为空
    /// </summary>
    public bool IsEmpty { get => totalCountTracker <= 0; }

    /// <summary>
    /// 计数五星物品
    /// </summary>
    /// <param name="item">物品</param>
    /// <returns>是否为Up物品</returns>
    public bool IncreaseOrange(IStatisticsItemConvertible item)
    {
        orangeCounter.IncreaseByOne(item);
        ++totalCountTracker;

        return orangeUpCounter.TryIncreaseByOne(item);
    }

    /// <summary>
    /// 计数四星物品
    /// </summary>
    /// <param name="item">物品</param>
    public void IncreasePurple(IStatisticsItemConvertible item)
    {
        purpleUpCounter.TryIncreaseByOne(item);
        purpleCounter.IncreaseByOne(item);
        ++totalCountTracker;
    }

    /// <summary>
    /// 计数三星武器
    /// </summary>
    /// <param name="item">武器</param>
    public void IncreaseBlue(IStatisticsItemConvertible item)
    {
        blueCounter.IncreaseByOne(item);
        ++totalCountTracker;
    }

    /// <summary>
    /// 转换到卡池历史记录
    /// </summary>
    /// <returns>卡池历史记录</returns>
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
            OrangeUpList = orangeUpCounter.ToStatisticsList(),
            PurpleUpList = purpleUpCounter.ToStatisticsList(),
            OrangeList = orangeCounter.ToStatisticsList(),
            PurpleList = purpleCounter.ToStatisticsList(),
            BlueList = blueCounter.ToStatisticsList(),
        };

        return historyWish;
    }
}