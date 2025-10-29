// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.Immutable;
using GachaItem = Snap.Hutao.Model.Entity.GachaItem;

namespace Snap.Hutao.Service.GachaLog.Factory;

[GeneratedConstructor]
[Service(ServiceLifetime.Singleton, typeof(IGachaStatisticsFactory))]
internal sealed partial class GachaStatisticsFactory : IGachaStatisticsFactory
{
    // We must use the root service provider, if not, PullPrediction will cause memory leak
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    /// <inheritdoc/>
    public async ValueTask<GachaStatistics> CreateAsync(GachaLogServiceMetadataContext metadata, ImmutableArray<GachaItem> items)
    {
        await taskContext.SwitchToBackgroundAsync();

        ImmutableArray<HistoryWishBuilder> historyWishBuilders = metadata.GachaEvents
            .SelectAsArray(static (gachaEvent, metadata) => HistoryWishBuilder.Create(gachaEvent, metadata), metadata);
        return SynchronizedCreate(new(serviceProvider, items, historyWishBuilders, metadata));
    }

    private static GachaStatistics SynchronizedCreate(GachaStatisticsFactoryContext context)
    {
        TypedWishSummaryBuilder standardWishBuilder = TypedWishSummaryBuilderContext.StandardWish(context).CreateBuilder();
        TypedWishSummaryBuilder avatarWishBuilder = TypedWishSummaryBuilderContext.AvatarEventWish(context).CreateBuilder();
        TypedWishSummaryBuilder weaponWishBuilder = TypedWishSummaryBuilderContext.WeaponEventWish(context).CreateBuilder();
        TypedWishSummaryBuilder chronicledWishBuilder = TypedWishSummaryBuilderContext.ChronicledWish(context).CreateBuilder();
        GachaStatisticsItemCounter itemCounter = new(context);

        // Pre group builders
        Dictionary<GachaType, List<HistoryWishBuilder>> historyWishBuilderMap = context.HistoryWishBuilders
            .GroupBy(b => b.ConfigType)
            .ToDictionary(g => g.Key, g => g.ToList().SortBy(b => b.From));

        // Items are ordered by precise time, first is oldest
        foreach (ref readonly GachaItem item in context.Items.AsSpan())
        {
            // Find target history wish to operate. // banner.From <= item.Time <= banner.To
            HistoryWishBuilder? targetHistoryWishBuilder = item.GachaType is not (GachaType.Standard or GachaType.NewBie)
                ? historyWishBuilderMap[item.GachaType].BinarySearch(item, static (pinned, banner) => pinned.Time < banner.From ? -1 : pinned.Time > banner.To ? 1 : 0)
                : default;

            switch (item.ItemId.StringLength())
            {
                case 8U:
                    {
                        Avatar avatar = context.Metadata.GetAvatar(item.ItemId);

                        bool isUp = false;
                        switch (avatar.Quality)
                        {
                            case QualityType.QUALITY_ORANGE:
                                itemCounter.OrangeAvatar.IncreaseByOne(avatar);
                                isUp = targetHistoryWishBuilder?.IncreaseOrange(avatar) ?? false;
                                break;
                            case QualityType.QUALITY_PURPLE:
                                itemCounter.PurpleAvatar.IncreaseByOne(avatar);
                                targetHistoryWishBuilder?.IncreasePurple(avatar);
                                break;
                        }

                        standardWishBuilder.Track(item, avatar, isUp);
                        avatarWishBuilder.Track(item, avatar, isUp);
                        weaponWishBuilder.Track(item, avatar, isUp);
                        chronicledWishBuilder.Track(item, avatar, isUp);
                        break;
                    }

                case 5U:
                    {
                        Weapon weapon = context.Metadata.GetWeapon(item.ItemId);

                        bool isUp = false;
                        switch (weapon.RankLevel)
                        {
                            case QualityType.QUALITY_ORANGE:
                                isUp = targetHistoryWishBuilder?.IncreaseOrange(weapon) ?? false;
                                itemCounter.OrangeWeapon.IncreaseByOne(weapon);
                                break;
                            case QualityType.QUALITY_PURPLE:
                                targetHistoryWishBuilder?.IncreasePurple(weapon);
                                itemCounter.PurpleWeapon.IncreaseByOne(weapon);
                                break;
                            case QualityType.QUALITY_BLUE:
                                targetHistoryWishBuilder?.IncreaseBlue(weapon);
                                itemCounter.BlueWeapon.IncreaseByOne(weapon);
                                break;
                        }

                        standardWishBuilder.Track(item, weapon, isUp);
                        avatarWishBuilder.Track(item, weapon, isUp);
                        weaponWishBuilder.Track(item, weapon, isUp);
                        chronicledWishBuilder.Track(item, weapon, isUp);
                        break;
                    }

                default:
                    // ItemId string length not correct.
                    HutaoException.GachaStatisticsInvalidItemId(item.ItemId);
                    break;
            }
        }

        AsyncBarrier barrier = new(4);

        ImmutableArray<HistoryWish> historyWishes =
        [
            .. context.HistoryWishBuilders
                .Where(b => context.IsEmptyHistoryWishVisible || !b.IsEmpty)
                .OrderByDescending(builder => builder.From)
                .ThenBy(builder => builder.ConfigType, GachaTypeComparer.Shared)
                .Select(builder => builder.ToHistoryWish())
        ];

        return new()
        {
            // History
            HistoryWishes = historyWishes.AsAdvancedCollectionView(),

            // Avatars
            OrangeAvatars = itemCounter.OrangeAvatar.ToStatisticsImmutableArray(),
            PurpleAvatars = itemCounter.PurpleAvatar.ToStatisticsImmutableArray(),

            // Weapons
            OrangeWeapons = itemCounter.OrangeWeapon.ToStatisticsImmutableArray(),
            PurpleWeapons = itemCounter.PurpleWeapon.ToStatisticsImmutableArray(),
            BlueWeapons = itemCounter.BlueWeapon.ToStatisticsImmutableArray(),

            // Typed wish summary
            StandardWish = standardWishBuilder.ToTypedWishSummary(barrier),
            AvatarWish = avatarWishBuilder.ToTypedWishSummary(barrier),
            WeaponWish = weaponWishBuilder.ToTypedWishSummary(barrier),
            ChronicledWish = chronicledWishBuilder.ToTypedWishSummary(barrier),
        };
    }
}