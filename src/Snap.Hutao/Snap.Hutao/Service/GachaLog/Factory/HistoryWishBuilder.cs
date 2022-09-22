// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Gacha;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.GachaLog.Factory;

/// <summary>
/// 卡池历史记录建造器
/// </summary>
internal class HistoryWishBuilder
{
    private readonly GachaEvent gachaEvent;
    private readonly GachaConfigType configType;

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
    /// <param name="nameAvatarMap">命名角色映射</param>
    /// <param name="nameWeaponMap">命名武器映射</param>
    public HistoryWishBuilder(GachaEvent gachaEvent, Dictionary<string, Avatar> nameAvatarMap, Dictionary<string, Weapon> nameWeaponMap)
    {
        this.gachaEvent = gachaEvent;
        configType = gachaEvent.Type;

        if (configType == GachaConfigType.AvatarEventWish || configType == GachaConfigType.AvatarEventWish2)
        {
            orangeUpCounter = gachaEvent.UpOrangeList.Select(name => nameAvatarMap[name]).ToDictionary(a => (IStatisticsItemSource)a, a => 0);
            purpleUpCounter = gachaEvent.UpPurpleList.Select(name => nameAvatarMap[name]).ToDictionary(a => (IStatisticsItemSource)a, a => 0);
        }
        else if (configType == GachaConfigType.WeaponEventWish)
        {
            orangeUpCounter = gachaEvent.UpOrangeList.Select(name => nameWeaponMap[name]).ToDictionary(w => (IStatisticsItemSource)w, w => 0);
            purpleUpCounter = gachaEvent.UpPurpleList.Select(name => nameWeaponMap[name]).ToDictionary(w => (IStatisticsItemSource)w, w => 0);
        }
    }

    /// <summary>
    /// 祈愿配置类型
    /// </summary>
    public GachaConfigType ConfigType { get => configType; }

    /// <inheritdoc cref="GachaEvent.From"/>
    public DateTimeOffset From
    {
        get => gachaEvent.From;
    }

    /// <inheritdoc cref="GachaEvent.To"/>
    public DateTimeOffset To
    {
        get => gachaEvent.To;
    }

    /// <summary>
    /// 计数五星角色
    /// </summary>
    /// <param name="avatar">角色</param>
    /// <returns>是否为Up角色</returns>
    public bool IncreaseOrangeAvatar(Avatar avatar)
    {
        orangeCounter.Increase(avatar);
        ++totalCountTracker;

        return orangeUpCounter.TryIncrease(avatar);
    }

    /// <summary>
    /// 计数四星角色
    /// </summary>
    /// <param name="avatar">角色</param>
    public void IncreasePurpleAvatar(Avatar avatar)
    {
        purpleUpCounter.TryIncrease(avatar);
        purpleCounter.Increase(avatar);
        ++totalCountTracker;
    }

    /// <summary>
    /// 计数五星武器
    /// </summary>
    /// <param name="weapon">武器</param>
    /// <returns>是否为Up武器</returns>
    public bool IncreaseOrangeWeapon(Weapon weapon)
    {
        orangeCounter.Increase(weapon);
        ++totalCountTracker;
        return orangeUpCounter.TryIncrease(weapon);
    }

    /// <summary>
    /// 计数四星武器
    /// </summary>
    /// <param name="weapon">武器</param>
    public void IncreasePurpleWeapon(Weapon weapon)
    {
        purpleUpCounter.TryIncrease(weapon);
        purpleCounter.Increase(weapon);
        ++totalCountTracker;
    }

    /// <summary>
    /// 计数三星武器
    /// </summary>
    /// <param name="weapon">武器</param>
    public void IncreaseBlueWeapon(Weapon weapon)
    {
        blueCounter.Increase(weapon);
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
            OrangeUpList = orangeUpCounter.ToStatisticsList(),
            PurpleUpList = purpleUpCounter.ToStatisticsList(),
            OrangeList = orangeCounter.ToStatisticsList(),
            PurpleList = purpleCounter.ToStatisticsList(),
            BlueList = blueCounter.ToStatisticsList(),
        };

        return historyWish;
    }
}