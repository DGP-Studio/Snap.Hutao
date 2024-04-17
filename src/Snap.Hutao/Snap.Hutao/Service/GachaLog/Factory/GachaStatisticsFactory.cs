// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Hutao.GachaLog;
using System.Collections.Frozen;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.GachaLog.Factory;

/// <summary>
/// 祈愿统计工厂
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IGachaStatisticsFactory))]
internal sealed partial class GachaStatisticsFactory : IGachaStatisticsFactory
{
    private static readonly FrozenSet<uint> BlueStandardWeaponIdsSet = FrozenSet.ToFrozenSet(
    [
        11301U, 11302U, 11306U, 12301U, 12302U, 12305U, 13303U, 14301U, 14302U, 14304U, 15301U, 15302U, 15304U
    ]);

    private static readonly FrozenSet<uint> PurpleStandardWeaponIdsSet = FrozenSet.ToFrozenSet(
    [
        11401U, 11402U, 11403U, 11405U, 12401U, 12402U, 12403U, 12405U, 13401U, 13407U, 14401U, 14402U, 14403U, 14409U, 15401U, 15402U, 15403U, 15405U
    ]);

    private readonly IMetadataService metadataService;
    private readonly HomaGachaLogClient homaGachaLogClient;
    private readonly ITaskContext taskContext;
    private readonly AppOptions options;

    /// <inheritdoc/>
    public async ValueTask<GachaStatistics> CreateAsync(List<Model.Entity.GachaItem> items, GachaLogServiceMetadataContext context)
    {
        await taskContext.SwitchToBackgroundAsync();

        List<HistoryWishBuilder> historyWishBuilders = context.GachaEvents.SelectList(gachaEvent => new HistoryWishBuilder(gachaEvent, context));
        return CreateCore(taskContext, homaGachaLogClient, items, historyWishBuilders, context, options.IsEmptyHistoryWishVisible, options.IsUnobtainedWishItemVisible);
    }

    private static GachaStatistics CreateCore(
        ITaskContext taskContext,
        HomaGachaLogClient gachaLogClient,
        List<Model.Entity.GachaItem> items,
        List<HistoryWishBuilder> historyWishBuilders,
        in GachaLogServiceMetadataContext context,
        bool isEmptyHistoryWishVisible,
        bool isNeverHeldStatisticsItemVisible)
    {
        TypedWishSummaryBuilderContext standardContext = TypedWishSummaryBuilderContext.StandardWish(taskContext, gachaLogClient);
        TypedWishSummaryBuilder standardWishBuilder = new(standardContext);

        TypedWishSummaryBuilderContext avatarContext = TypedWishSummaryBuilderContext.AvatarEventWish(taskContext, gachaLogClient);
        TypedWishSummaryBuilder avatarWishBuilder = new(avatarContext);

        TypedWishSummaryBuilderContext weaponContext = TypedWishSummaryBuilderContext.WeaponEventWish(taskContext, gachaLogClient);
        TypedWishSummaryBuilder weaponWishBuilder = new(weaponContext);

        TypedWishSummaryBuilderContext chronicledContext = TypedWishSummaryBuilderContext.ChronicledWish(taskContext, gachaLogClient);
        TypedWishSummaryBuilder chronicledWishBuilder = new(chronicledContext);

        Dictionary<Avatar, int> orangeAvatarCounter = [];
        Dictionary<Avatar, int> purpleAvatarCounter = [];
        Dictionary<Weapon, int> orangeWeaponCounter = [];
        Dictionary<Weapon, int> purpleWeaponCounter = [];
        Dictionary<Weapon, int> blueWeaponCounter = [];

        if (isNeverHeldStatisticsItemVisible)
        {
            orangeAvatarCounter = context.IdAvatarMap.Values
                .Where(avatar => avatar.Quality == QualityType.QUALITY_ORANGE)
                .ToDictionary(avatar => avatar, _ => 0);
            purpleAvatarCounter = context.IdAvatarMap.Values
               .Where(avatar => avatar.Quality == QualityType.QUALITY_PURPLE)
               .ToDictionary(avatar => avatar, _ => 0);
            orangeWeaponCounter = context.IdWeaponMap.Values
               .Where(weapon => weapon.Quality == QualityType.QUALITY_ORANGE)
               .ToDictionary(weapon => weapon, _ => 0);

            HashSet<Weapon> purpleWeapons = [];
            foreach (uint weaponId in PurpleStandardWeaponIdsSet)
            {
                purpleWeapons.Add(context.IdWeaponMap[weaponId]);
            }

            foreach (GachaEvent gachaEvent in context.GachaEvents)
            {
                if (gachaEvent.Type is GachaType.ActivityWeapon)
                {
                    foreach (uint weaponId in gachaEvent.UpPurpleList)
                    {
                        purpleWeapons.Add(context.IdWeaponMap[weaponId]);
                    }
                }
            }

            HashSet<Weapon> blueWeapons = [];
            foreach (uint weaponId in BlueStandardWeaponIdsSet)
            {
                blueWeapons.Add(context.IdWeaponMap[weaponId]);
            }

            purpleWeaponCounter = purpleWeapons.ToDictionary(weapon => weapon, _ => 0);
            blueWeaponCounter = blueWeapons.ToDictionary(weapon => weapon, _ => 0);
        }

        // Pre group builders
        Dictionary<GachaType, List<HistoryWishBuilder>> historyWishBuilderMap = historyWishBuilders
            .GroupBy(b => b.ConfigType)
            .ToDictionary(g => g.Key, g => g.ToList().SortBy(b => b.From));

        // Items are ordered by precise time, first is oldest
        // 'ref' is not allowed here because we have lambda below
        foreach (ref readonly Model.Entity.GachaItem item in CollectionsMarshal.AsSpan(items))
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
                        Avatar avatar = context.GetAvatar(item.ItemId);

                        bool isUp = false;
                        switch (avatar.Quality)
                        {
                            case QualityType.QUALITY_ORANGE:
                                orangeAvatarCounter.IncreaseOne(avatar);
                                isUp = targetHistoryWishBuilder?.IncreaseOrange(avatar) ?? false;
                                break;
                            case QualityType.QUALITY_PURPLE:
                                purpleAvatarCounter.IncreaseOne(avatar);
                                targetHistoryWishBuilder?.IncreasePurple(avatar);
                                break;
                            default:
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
                        Weapon weapon = context.IdWeaponMap[item.ItemId];

                        bool isUp = false;
                        switch (weapon.RankLevel)
                        {
                            case QualityType.QUALITY_ORANGE:
                                isUp = targetHistoryWishBuilder?.IncreaseOrange(weapon) ?? false;
                                orangeWeaponCounter.IncreaseOne(weapon);
                                break;
                            case QualityType.QUALITY_PURPLE:
                                targetHistoryWishBuilder?.IncreasePurple(weapon);
                                purpleWeaponCounter.IncreaseOne(weapon);
                                break;
                            case QualityType.QUALITY_BLUE:
                                targetHistoryWishBuilder?.IncreaseBlue(weapon);
                                blueWeaponCounter.IncreaseOne(weapon);
                                break;
                            default:
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

        return new()
        {
            // history
            HistoryWishes = historyWishBuilders
                .Where(b => isEmptyHistoryWishVisible || (!b.IsEmpty))
                .OrderByDescending(builder => builder.From)
                .ThenBy(builder => builder.ConfigType, GachaTypeComparer.Shared)
                .Select(builder => builder.ToHistoryWish())
                .ToList(),

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