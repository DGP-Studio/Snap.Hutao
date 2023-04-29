// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.ViewModel.GachaLog;

namespace Snap.Hutao.Service.GachaLog.Factory;

/// <summary>
/// 祈愿统计工厂
/// </summary>
[HighQuality]
[Injection(InjectAs.Scoped, typeof(IGachaStatisticsFactory))]
internal sealed class GachaStatisticsFactory : IGachaStatisticsFactory
{
    private readonly IMetadataService metadataService;
    private readonly AppOptions options;

    /// <summary>
    /// 构造一个新的祈愿统计工厂
    /// </summary>
    /// <param name="metadataService">元数据服务</param>
    /// <param name="options">选项</param>
    public GachaStatisticsFactory(IMetadataService metadataService, AppOptions options)
    {
        this.metadataService = metadataService;
        this.options = options;
    }

    /// <inheritdoc/>
    public async Task<GachaStatistics> CreateAsync(IOrderedQueryable<GachaItem> items, GachaLogServiceContext context)
    {
        List<GachaEvent> gachaEvents = await metadataService.GetGachaEventsAsync().ConfigureAwait(false);
        List<HistoryWishBuilder> historyWishBuilders = gachaEvents.SelectList(g => new HistoryWishBuilder(g, context));

        return CreateCore(items, historyWishBuilders, context, options.IsEmptyHistoryWishVisible);
    }

    private static GachaStatistics CreateCore(
        IOrderedQueryable<GachaItem> items,
        List<HistoryWishBuilder> historyWishBuilders,
        in GachaLogServiceContext context,
        bool isEmptyHistoryWishVisible)
    {
        TypedWishSummaryBuilder standardWishBuilder = new(SH.ServiceGachaLogFactoryPermanentWishName, TypedWishSummaryBuilder.IsStandardWish, 90, 10);
        TypedWishSummaryBuilder avatarWishBuilder = new(SH.ServiceGachaLogFactoryAvatarWishName, TypedWishSummaryBuilder.IsAvatarEventWish, 90, 10);
        TypedWishSummaryBuilder weaponWishBuilder = new(SH.ServiceGachaLogFactoryWeaponWishName, TypedWishSummaryBuilder.IsWeaponEventWish, 80, 10);

        Dictionary<Avatar, int> orangeAvatarCounter = new();
        Dictionary<Avatar, int> purpleAvatarCounter = new();
        Dictionary<Weapon, int> orangeWeaponCounter = new();
        Dictionary<Weapon, int> purpleWeaponCounter = new();
        Dictionary<Weapon, int> blueWeaponCounter = new();

        // Items are ordered by precise time
        // first is oldest
        foreach (GachaItem item in items)
        {
            // Find target history wish to operate.
            HistoryWishBuilder? targetHistoryWishBuilder = historyWishBuilders
                .Where(w => w.ConfigType == item.GachaType)
                .SingleOrDefault(w => w.From <= item.Time && w.To >= item.Time);

            // It's an avatar
            switch (item.ItemId.Place())
            {
                case 8:
                    {
                        Avatar avatar = context.IdAvatarMap[item.ItemId];

                        bool isUp = false;
                        switch (avatar.Quality)
                        {
                            case ItemQuality.QUALITY_ORANGE:
                                orangeAvatarCounter.Increase(avatar);
                                isUp = targetHistoryWishBuilder?.IncreaseOrange(avatar) ?? false;
                                break;
                            case ItemQuality.QUALITY_PURPLE:
                                purpleAvatarCounter.Increase(avatar);
                                targetHistoryWishBuilder?.IncreasePurple(avatar);
                                break;
                        }

                        standardWishBuilder.Track(item, avatar, isUp);
                        avatarWishBuilder.Track(item, avatar, isUp);
                        weaponWishBuilder.Track(item, avatar, isUp);
                        break;
                    }

                case 5:
                    {
                        Weapon weapon = context.IdWeaponMap[item.ItemId];

                        bool isUp = false;
                        switch (weapon.RankLevel)
                        {
                            case ItemQuality.QUALITY_ORANGE:
                                isUp = targetHistoryWishBuilder?.IncreaseOrange(weapon) ?? false;
                                orangeWeaponCounter.Increase(weapon);
                                break;
                            case ItemQuality.QUALITY_PURPLE:
                                targetHistoryWishBuilder?.IncreasePurple(weapon);
                                purpleWeaponCounter.Increase(weapon);
                                break;
                            case ItemQuality.QUALITY_BLUE:
                                targetHistoryWishBuilder?.IncreaseBlue(weapon);
                                blueWeaponCounter.Increase(weapon);
                                break;
                        }

                        standardWishBuilder.Track(item, weapon, isUp);
                        avatarWishBuilder.Track(item, weapon, isUp);
                        weaponWishBuilder.Track(item, weapon, isUp);
                        break;
                    }

                default:
                    // ItemId place not correct.
                    ThrowHelper.UserdataCorrupted(string.Format(SH.ServiceGachaStatisticsFactoryItemIdInvalid, item.ItemId), null!);
                    break;
            }
        }

        return new()
        {
            // history
            HistoryWishes = historyWishBuilders
                .Where(b => isEmptyHistoryWishVisible || (!b.IsEmpty))
                .OrderByDescending(builder => builder.From)
                .ThenBy(builder => builder.ConfigType, GachaConfigTypeComparar.Shared)
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
            StandardWish = standardWishBuilder.ToTypedWishSummary(),
            AvatarWish = avatarWishBuilder.ToTypedWishSummary(),
            WeaponWish = weaponWishBuilder.ToTypedWishSummary(),
        };
    }
}