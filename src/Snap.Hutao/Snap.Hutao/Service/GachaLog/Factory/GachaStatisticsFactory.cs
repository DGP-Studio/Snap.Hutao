// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Hutao.GachaLog;
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
    private readonly IMetadataService metadataService;
    private readonly HomaGachaLogClient homaGachaLogClient;
    private readonly ITaskContext taskContext;
    private readonly AppOptions options;

    /// <inheritdoc/>
    public async ValueTask<GachaStatistics> CreateAsync(List<Model.Entity.GachaItem> items, GachaLogServiceMetadataContext context)
    {
        await taskContext.SwitchToBackgroundAsync();
        List<GachaEvent> gachaEvents = await metadataService.GetGachaEventsAsync().ConfigureAwait(false);
        List<HistoryWishBuilder> historyWishBuilders = gachaEvents.SelectList(gachaEvent => new HistoryWishBuilder(gachaEvent, context));

        return CreateCore(taskContext, homaGachaLogClient, items, historyWishBuilders, context, options.IsEmptyHistoryWishVisible);
    }

    private static GachaStatistics CreateCore(
        ITaskContext taskContext,
        HomaGachaLogClient gachaLogClient,
        List<Model.Entity.GachaItem> items,
        List<HistoryWishBuilder> historyWishBuilders,
        in GachaLogServiceMetadataContext context,
        bool isEmptyHistoryWishVisible)
    {
        TypedWishSummaryBuilderContext standardContext = TypedWishSummaryBuilderContext.StandardWish(taskContext, gachaLogClient);
        TypedWishSummaryBuilder standardWishBuilder = new(standardContext);

        TypedWishSummaryBuilderContext avatarContext = TypedWishSummaryBuilderContext.AvatarEventWish(taskContext, gachaLogClient);
        TypedWishSummaryBuilder avatarWishBuilder = new(avatarContext);

        TypedWishSummaryBuilderContext weaponContext = TypedWishSummaryBuilderContext.WeaponEventWish(taskContext, gachaLogClient);
        TypedWishSummaryBuilder weaponWishBuilder = new(weaponContext);

        Dictionary<Avatar, int> orangeAvatarCounter = new();
        Dictionary<Avatar, int> purpleAvatarCounter = new();
        Dictionary<Weapon, int> orangeWeaponCounter = new();
        Dictionary<Weapon, int> purpleWeaponCounter = new();
        Dictionary<Weapon, int> blueWeaponCounter = new();

        // Pre group builders
        Dictionary<GachaConfigType, List<HistoryWishBuilder>> historyWishBuilderMap = historyWishBuilders
            .GroupBy(b => b.ConfigType)
            .ToDictionary(g => g.Key, g => g.ToList().SortBy(b => b.From));

        // Items are ordered by precise time, first is oldest
        // 'ref' is not allowed here because we have lambda below
        foreach (Model.Entity.GachaItem item in CollectionsMarshal.AsSpan(items))
        {
            // Find target history wish to operate. // w.From <= item.Time <= w.To
            HistoryWishBuilder? targetHistoryWishBuilder = item.GachaType is not (GachaConfigType.StandardWish or GachaConfigType.NoviceWish)
                ? historyWishBuilderMap[item.GachaType].BinarySearch(w => item.Time < w.From ? -1 : item.Time > w.To ? 1 : 0)
                : default;

            switch (item.ItemId.StringLength())
            {
                case 8U:
                    {
                        Avatar avatar = context.IdAvatarMap[item.ItemId];

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
                        break;
                    }

                default:
                    // ItemId string length not correct.
                    ThrowHelper.UserdataCorrupted(SH.ServiceGachaStatisticsFactoryItemIdInvalid.Format(item.ItemId), default!);
                    break;
            }
        }

        AsyncBarrier barrier = new(3);

        return new()
        {
            // history
            HistoryWishes = historyWishBuilders
                .Where(b => isEmptyHistoryWishVisible || (!b.IsEmpty))
                .OrderByDescending(builder => builder.From)
                .ThenBy(builder => builder.ConfigType, GachaConfigTypeComparer.Shared)
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
        };
    }
}