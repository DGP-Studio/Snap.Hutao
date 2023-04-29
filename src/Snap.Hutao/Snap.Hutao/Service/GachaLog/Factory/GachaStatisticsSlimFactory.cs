// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.GachaLog.Factory;

/// <summary>
/// 简化的祈愿统计工厂
/// </summary>
[Injection(InjectAs.Scoped, typeof(IGachaStatisticsSlimFactory))]
internal sealed class GachaStatisticsSlimFactory : IGachaStatisticsSlimFactory
{
    private readonly ITaskContext taskContext;

    /// <summary>
    /// 构造一个新的简化的祈愿统计工厂
    /// </summary>
    /// <param name="taskContext">任务上下文</param>
    public GachaStatisticsSlimFactory(ITaskContext taskContext)
    {
        this.taskContext = taskContext;
    }

    /// <inheritdoc/>
    public async Task<GachaStatisticsSlim> CreateAsync(IOrderedQueryable<GachaItem> items, GachaLogServiceContext context)
    {
        await taskContext.SwitchToBackgroundAsync();

        int standardOrangeTracker = 0;
        int standardPurpleTracker = 0;
        TypedWishSummarySlim standardWish = new(SH.ServiceGachaLogFactoryPermanentWishName, 90, 10);

        int avatarOrangeTracker = 0;
        int avatarPurpleTracker = 0;
        TypedWishSummarySlim avatarWish = new(SH.ServiceGachaLogFactoryAvatarWishName, 90, 10);

        int weaponOrangeTracker = 0;
        int weaponPurpleTracker = 0;
        TypedWishSummarySlim weaponWish = new(SH.ServiceGachaLogFactoryWeaponWishName, 80, 10);

        // O(n) operation
        foreach (GachaItem item in items)
        {
            INameQuality nameQuality = context.GetNameQualityByItemId(item.ItemId);
            switch (item.QueryType)
            {
                case GachaConfigType.StandardWish:
                    Track(nameQuality, ref standardOrangeTracker, ref standardPurpleTracker);
                    break;
                case GachaConfigType.AvatarEventWish:
                case GachaConfigType.AvatarEventWish2:
                    Track(nameQuality, ref avatarOrangeTracker, ref avatarPurpleTracker);
                    break;
                case GachaConfigType.WeaponEventWish:
                    Track(nameQuality, ref weaponOrangeTracker, ref weaponPurpleTracker);
                    break;
            }
        }

        standardWish.LastOrangePull = standardOrangeTracker;
        standardWish.LastPurplePull = standardPurpleTracker;
        avatarWish.LastOrangePull = avatarOrangeTracker;
        avatarWish.LastPurplePull = avatarPurpleTracker;
        weaponWish.LastOrangePull = weaponOrangeTracker;
        weaponWish.LastPurplePull = weaponPurpleTracker;

        return new()
        {
            AvatarWish = avatarWish,
            WeaponWish = weaponWish,
            StandardWish = standardWish,
        };
    }

    private static void Track(INameQuality nameQuality, ref int orangeTracker, ref int purpleTracker)
    {
        switch (nameQuality.Quality)
        {
            case ItemQuality.QUALITY_ORANGE:
                orangeTracker = 0;
                ++purpleTracker;
                break;
            case ItemQuality.QUALITY_PURPLE:
                ++orangeTracker;
                purpleTracker = 0;
                break;
            case ItemQuality.QUALITY_BLUE:
                ++orangeTracker;
                ++purpleTracker;
                break;
        }
    }
}