// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Hutao.GachaLog;
using System.Collections.Frozen;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.GachaLog.Factory;

[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IGachaStatisticsFactory))]
internal sealed partial class GachaStatisticsFactory : IGachaStatisticsFactory
{
    private static readonly FrozenSet<uint> BlueStandardWeaponIdsSet =
    [
        11301U, 11302U, 11306U,
        12301U, 12302U, 12305U,
        13303U,
        14301U, 14302U, 14304U,
        15301U, 15302U, 15304U
    ];

    private static readonly FrozenSet<uint> PurpleStandardWeaponIdsSet =
    [
        11401U, 11402U, 11403U, 11405U,
        12401U, 12402U, 12403U, 12405U,
        13401U, 13407U,
        14401U, 14402U, 14403U, 14409U,
        15401U, 15402U, 15403U, 15405U
    ];

    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly AppOptions options;

    /// <inheritdoc/>
    public async ValueTask<GachaStatistics> CreateAsync(List<Model.Entity.GachaItem> items, GachaLogServiceMetadataContext metadata)
    {
        await taskContext.SwitchToBackgroundAsync();

        List<HistoryWishBuilder> historyWishBuilders = metadata.GachaEvents.Select(gachaEvent => new HistoryWishBuilder(gachaEvent, metadata)).ToList();

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HomaGachaLogClient homaGachaLogClient = scope.ServiceProvider.GetRequiredService<HomaGachaLogClient>();
            GachaStatisticsFactoryContext context = new(taskContext, homaGachaLogClient, items, historyWishBuilders, metadata, options);
            return CreateCore(context);
        }
    }

    private static GachaStatistics CreateCore(GachaStatisticsFactoryContext context)
    {
        TypedWishSummaryBuilderContext standardContext = TypedWishSummaryBuilderContext.StandardWish(context.TaskContext, context.GachaLogClient);
        TypedWishSummaryBuilder standardWishBuilder = new(standardContext);

        TypedWishSummaryBuilderContext avatarContext = TypedWishSummaryBuilderContext.AvatarEventWish(context.TaskContext, context.GachaLogClient);
        TypedWishSummaryBuilder avatarWishBuilder = new(avatarContext);

        TypedWishSummaryBuilderContext weaponContext = TypedWishSummaryBuilderContext.WeaponEventWish(context.TaskContext, context.GachaLogClient);
        TypedWishSummaryBuilder weaponWishBuilder = new(weaponContext);

        TypedWishSummaryBuilderContext chronicledContext = TypedWishSummaryBuilderContext.ChronicledWish(context.TaskContext, context.GachaLogClient);
        TypedWishSummaryBuilder chronicledWishBuilder = new(chronicledContext);

        Dictionary<Avatar, int> orangeAvatarCounter = [];
        Dictionary<Avatar, int> purpleAvatarCounter = [];
        Dictionary<Weapon, int> orangeWeaponCounter = [];
        Dictionary<Weapon, int> purpleWeaponCounter = [];
        Dictionary<Weapon, int> blueWeaponCounter = [];

        if (context.IsUnobtainedWishItemVisible)
        {
            orangeAvatarCounter = context.Metadata.IdAvatarMap.Values
                .Where(avatar => avatar.Quality == QualityType.QUALITY_ORANGE)
                .ToDictionary(avatar => avatar, _ => 0);
            purpleAvatarCounter = context.Metadata.IdAvatarMap.Values
               .Where(avatar => avatar.Quality == QualityType.QUALITY_PURPLE)
               .ToDictionary(avatar => avatar, _ => 0);
            orangeWeaponCounter = context.Metadata.IdWeaponMap.Values
               .Where(weapon => weapon.Quality == QualityType.QUALITY_ORANGE)
               .ToDictionary(weapon => weapon, _ => 0);

            HashSet<Weapon> purpleWeapons = [];
            foreach (uint weaponId in PurpleStandardWeaponIdsSet)
            {
                purpleWeapons.Add(context.Metadata.GetWeapon(weaponId));
            }

            foreach (GachaEvent gachaEvent in context.Metadata.GachaEvents)
            {
                if (gachaEvent.Type is GachaType.ActivityWeapon)
                {
                    foreach (uint weaponId in gachaEvent.UpPurpleList)
                    {
                        purpleWeapons.Add(context.Metadata.GetWeapon(weaponId));
                    }
                }
            }

            HashSet<Weapon> blueWeapons = [];
            foreach (uint weaponId in BlueStandardWeaponIdsSet)
            {
                blueWeapons.Add(context.Metadata.GetWeapon(weaponId));
            }

            purpleWeaponCounter = purpleWeapons.ToDictionary(weapon => weapon, _ => 0);
            blueWeaponCounter = blueWeapons.ToDictionary(weapon => weapon, _ => 0);
        }

        // Pre group builders
        Dictionary<GachaType, List<HistoryWishBuilder>> historyWishBuilderMap = context.HistoryWishBuilders
            .GroupBy(b => b.ConfigType)
            .ToDictionary(g => g.Key, g => g.ToList().SortBy(b => b.From));

        // Items are ordered by precise time, first is oldest
        foreach (ref readonly Model.Entity.GachaItem item in CollectionsMarshal.AsSpan(context.Items))
        {
            // Find target history wish to operate. // banner.From <= item.Time <= banner.To
            Model.Entity.GachaItem pinned = item;
            HistoryWishBuilder? targetHistoryWishBuilder = item.GachaType is not (GachaType.Standard or GachaType.NewBie)
                ? historyWishBuilderMap[item.GachaType].BinarySearch(banner => pinned.Time < banner.From ? -1 : pinned.Time > banner.To ? 1 : 0)
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
                                orangeAvatarCounter.IncreaseByOne(avatar);
                                isUp = targetHistoryWishBuilder?.IncreaseOrange(avatar) ?? false;
                                break;
                            case QualityType.QUALITY_PURPLE:
                                purpleAvatarCounter.IncreaseByOne(avatar);
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
                                orangeWeaponCounter.IncreaseByOne(weapon);
                                break;
                            case QualityType.QUALITY_PURPLE:
                                targetHistoryWishBuilder?.IncreasePurple(weapon);
                                purpleWeaponCounter.IncreaseByOne(weapon);
                                break;
                            case QualityType.QUALITY_BLUE:
                                targetHistoryWishBuilder?.IncreaseBlue(weapon);
                                blueWeaponCounter.IncreaseByOne(weapon);
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

        List<HistoryWish> historyWishes = context.HistoryWishBuilders
            .Where(b => context.IsEmptyHistoryWishVisible || !b.IsEmpty)
            .OrderByDescending(builder => builder.From)
            .ThenBy(builder => builder.ConfigType, GachaTypeComparer.Shared)
            .Select(builder => builder.ToHistoryWish())
            .ToList();

        return new()
        {
            // history
            HistoryWishes = historyWishes.ToAdvancedCollectionView(),

            // avatars
            OrangeAvatars = orangeAvatarCounter.ToStatisticsList(),
            PurpleAvatars = purpleAvatarCounter.ToStatisticsList(),

            // weapons
            OrangeWeapons = orangeWeaponCounter.ToStatisticsList(),
            PurpleWeapons = purpleWeaponCounter.ToStatisticsList(),
            BlueWeapons = blueWeaponCounter.ToStatisticsList(),

            // typed wish summary
            StandardWish = standardWishBuilder.ToTypedWishSummary(barrier),
            AvatarWish = avatarWishBuilder.ToTypedWishSummary(barrier),
            WeaponWish = weaponWishBuilder.ToTypedWishSummary(barrier),
            ChronicledWish = chronicledWishBuilder.ToTypedWishSummary(barrier),
        };
    }
}