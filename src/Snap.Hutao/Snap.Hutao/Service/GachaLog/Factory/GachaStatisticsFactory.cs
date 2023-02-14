// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Extension;
using Snap.Hutao.Model.Binding.Gacha;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata;

namespace Snap.Hutao.Service.GachaLog.Factory;

/// <summary>
/// 祈愿统计工厂
/// </summary>
[Injection(InjectAs.Scoped, typeof(IGachaStatisticsFactory))]
internal class GachaStatisticsFactory : IGachaStatisticsFactory
{
    private readonly IMetadataService metadataService;
    private readonly AppDbContext appDbContext;

    /// <summary>
    /// 构造一个新的祈愿统计工厂
    /// </summary>
    /// <param name="metadataService">元数据服务</param>
    /// <param name="appDbContext">数据库上下文</param>
    public GachaStatisticsFactory(IMetadataService metadataService, AppDbContext appDbContext)
    {
        this.metadataService = metadataService;
        this.appDbContext = appDbContext;
    }

    /// <inheritdoc/>
    public async Task<GachaStatistics> CreateAsync(IEnumerable<GachaItem> items)
    {
        Dictionary<AvatarId, Avatar> idAvatarMap = await metadataService.GetIdToAvatarMapAsync().ConfigureAwait(false);
        Dictionary<WeaponId, Weapon> idWeaponMap = await metadataService.GetIdToWeaponMapAsync().ConfigureAwait(false);

        Dictionary<string, Avatar> nameAvatarMap = await metadataService.GetNameToAvatarMapAsync().ConfigureAwait(false);
        Dictionary<string, Weapon> nameWeaponMap = await metadataService.GetNameToWeaponMapAsync().ConfigureAwait(false);

        List<GachaEvent> gachaevents = await metadataService.GetGachaEventsAsync().ConfigureAwait(false);

        List<HistoryWishBuilder> historyWishBuilders = gachaevents.Select(g => new HistoryWishBuilder(g, nameAvatarMap, nameWeaponMap)).ToList();

        SettingEntry entry = await appDbContext.Settings
            .SingleOrAddAsync(SettingEntry.IsEmptyHistoryWishVisible, Core.StringLiterals.True)
            .ConfigureAwait(false);
        bool isEmptyHistoryWishVisible = entry.GetBoolean();

        IOrderedEnumerable<GachaItem> orderedItems = items.OrderBy(i => i.Id);
        await ThreadHelper.SwitchToBackgroundAsync();
        return CreateCore(orderedItems, historyWishBuilders, idAvatarMap, idWeaponMap, isEmptyHistoryWishVisible);
    }

    private static GachaStatistics CreateCore(
        IOrderedEnumerable<GachaItem> items,
        List<HistoryWishBuilder> historyWishBuilders,
        Dictionary<AvatarId, Avatar> avatarMap,
        Dictionary<WeaponId, Weapon> weaponMap,
        bool isEmptyHistoryWishVisible)
    {
        TypedWishSummaryBuilder standardWishBuilder = new(SH.ServiceGachaLogFactoryPermanentWishName, TypedWishSummaryBuilder.IsPermanentWish, 90, 10);
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
            if (item.ItemId.Place() == 8)
            {
                Avatar avatar = avatarMap[item.ItemId];

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
            }

            // It's a weapon
            else if (item.ItemId.Place() == 5)
            {
                Weapon weapon = weaponMap[item.ItemId];

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
            }
            else
            {
                // ItemId place not correct.
                // TODO: check items id when importing
                ThrowHelper.UserdataCorrupted(string.Format(SH.ServiceGachaStatisticsFactoryItemIdInvalid, item.ItemId), null!);
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
