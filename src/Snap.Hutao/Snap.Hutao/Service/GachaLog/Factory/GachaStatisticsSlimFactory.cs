// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.GachaLog.Factory;

[Service(ServiceLifetime.Scoped, typeof(IGachaStatisticsSlimFactory))]
internal sealed partial class GachaStatisticsSlimFactory : IGachaStatisticsSlimFactory
{
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial GachaStatisticsSlimFactory(IServiceProvider serviceProvider);

    public async ValueTask<GachaStatisticsSlim> CreateAsync(GachaLogServiceMetadataContext context, ImmutableArray<GachaItem> items, string uid)
    {
        await taskContext.SwitchToBackgroundAsync();
        return SynchronizedCreate(context, items, uid);
    }

    private static void Track(INameQualityAccess nameQuality, ref int orangeTracker, ref int purpleTracker)
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
        }
    }

    private static GachaStatisticsSlim SynchronizedCreate(in GachaLogServiceMetadataContext context, ImmutableArray<GachaItem> items, string uid)
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

        int chronicledOrangeTracker = 0;
        int chronicledPurpleTracker = 0;
        TypedWishSummarySlim chronicledWish = new(SH.ServiceGachaLogFactoryChronicledWishName, 90, 10);

        // O(n) operation
        foreach (ref readonly GachaItem item in items.AsSpan())
        {
            INameQualityAccess nameQuality = context.GetNameQualityByItemId(item.ItemId);
            switch (item.QueryType)
            {
                case GachaType.Standard:
                    Track(nameQuality, ref standardOrangeTracker, ref standardPurpleTracker);
                    break;
                case GachaType.ActivityAvatar:
                case GachaType.SpecialActivityAvatar:
                    Track(nameQuality, ref avatarOrangeTracker, ref avatarPurpleTracker);
                    break;
                case GachaType.ActivityWeapon:
                    Track(nameQuality, ref weaponOrangeTracker, ref weaponPurpleTracker);
                    break;
                case GachaType.ActivityCity:
                    Track(nameQuality, ref chronicledOrangeTracker, ref chronicledPurpleTracker);
                    break;
            }
        }

        standardWish.LastOrangePull = standardOrangeTracker;
        standardWish.LastPurplePull = standardPurpleTracker;

        avatarWish.LastOrangePull = avatarOrangeTracker;
        avatarWish.LastPurplePull = avatarPurpleTracker;

        weaponWish.LastOrangePull = weaponOrangeTracker;
        weaponWish.LastPurplePull = weaponPurpleTracker;

        chronicledWish.LastOrangePull = chronicledOrangeTracker;
        chronicledWish.LastPurplePull = chronicledPurpleTracker;

        return new()
        {
            Uid = uid,
            AvatarWish = avatarWish,
            WeaponWish = weaponWish,
            StandardWish = standardWish,
        };
    }
}