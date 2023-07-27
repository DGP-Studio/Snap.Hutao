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

    private readonly Dictionary<IStatisticsItemSource, int> orangeUpCounter = new();
    private readonly Dictionary<IStatisticsItemSource, int> purpleUpCounter = new();
    private readonly Dictionary<IStatisticsItemSource, int> orangeCounter = new();
    private readonly Dictionary<IStatisticsItemSource, int> purpleCounter = new();
    private readonly Dictionary<IStatisticsItemSource, int> blueCounter = new();

    private int totalCountTracker;

    /// <summary>
    /// 构造一个新的卡池历史记录建造器
    /// </summary>
    /// <param name="gachaEvent">卡池配置</param>
    /// <param name="context">祈愿记录上下文</param>
    [SuppressMessage("", "SH002")]
    public HistoryWishBuilder(GachaEvent gachaEvent, GachaLogServiceMetadataContext context)
    {
        this.gachaEvent = gachaEvent;
        ConfigType = gachaEvent.Type;

        switch (ConfigType)
        {
            case GachaConfigType.AvatarEventWish or GachaConfigType.AvatarEventWish2:
                orangeUpCounter = gachaEvent.UpOrangeList.Select(id => context.IdAvatarMap[id]).ToDictionary(a => (IStatisticsItemSource)a, a => 0);
                purpleUpCounter = gachaEvent.UpPurpleList.Select(id => context.IdAvatarMap[id]).ToDictionary(a => (IStatisticsItemSource)a, a => 0);
                break;
            case GachaConfigType.WeaponEventWish:
                orangeUpCounter = gachaEvent.UpOrangeList.Select(id => context.IdWeaponMap[id]).ToDictionary(w => (IStatisticsItemSource)w, w => 0);
                purpleUpCounter = gachaEvent.UpPurpleList.Select(id => context.IdWeaponMap[id]).ToDictionary(w => (IStatisticsItemSource)w, w => 0);
                break;
        }
    }

    /// <summary>
    /// 祈愿配置类型
    /// </summary>
    public GachaConfigType ConfigType { get; }

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
    public bool IncreaseOrange(IStatisticsItemSource item)
    {
        orangeCounter.IncreaseOne(item);
        ++totalCountTracker;

        return orangeUpCounter.TryIncreaseOne(item);
    }

    /// <summary>
    /// 计数四星物品
    /// </summary>
    /// <param name="item">物品</param>
    public void IncreasePurple(IStatisticsItemSource item)
    {
        purpleUpCounter.TryIncreaseOne(item);
        purpleCounter.IncreaseOne(item);
        ++totalCountTracker;
    }

    /// <summary>
    /// 计数三星武器
    /// </summary>
    /// <param name="item">武器</param>
    public void IncreaseBlue(IStatisticsItemSource item)
    {
        blueCounter.IncreaseOne(item);
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
            // base
            Name = gachaEvent.Name,
            From = gachaEvent.From,
            To = gachaEvent.To,
            TotalCount = totalCountTracker,

            // fill
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