// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.GachaLog.Factory;

/// <summary>
/// 简化的祈愿统计工厂
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IGachaStatisticsSlimFactory))]
internal sealed partial class GachaStatisticsSlimFactory : IGachaStatisticsSlimFactory
{
    private readonly ITaskContext taskContext;

    /// <inheritdoc/>
    public async ValueTask<GachaStatisticsSlim> CreateAsync(GachaLogServiceMetadataContext context, List<GachaItem> items, string uid)
    {
        await taskContext.SwitchToBackgroundAsync();

        return CreateCore(context, items, uid);
    }

    private static void Track(INameQuality nameQuality, ref int orangeTracker, ref int purpleTracker)
    {
        switch (nameQuality.Quality)
        {
            case QualityType.QUALITY_ORANGE:
                orangeTracker = 0;
                ++purpleTracker;
                break;
            case QualityType.QUALITY_PURPLE:
                ++orangeTracker;
                purpleTracker = 0;
                break;
            case QualityType.QUALITY_BLUE:
                ++orangeTracker;
                ++purpleTracker;
                break;
            default:
                break;
        }
    }

    private static GachaStatisticsSlim CreateCore(GachaLogServiceMetadataContext context, List<GachaItem> items, string uid)
    {
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
        foreach (ref readonly GachaItem item in CollectionsMarshal.AsSpan(items))
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
                default:
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
            Uid = uid,
            AvatarWish = avatarWish,
            WeaponWish = weaponWish,
            StandardWish = standardWish,
        };
    }
}