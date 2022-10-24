// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Extension;
using Snap.Hutao.Model.Binding.Gacha;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Service.Metadata;

namespace Snap.Hutao.Service.GachaLog.Factory;

/// <summary>
/// 祈愿统计工厂
/// </summary>
[Injection(InjectAs.Transient, typeof(IGachaStatisticsFactory))]
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
        Dictionary<int, Avatar> idAvatarMap = await metadataService.GetIdToAvatarMapAsync().ConfigureAwait(false);
        Dictionary<int, Weapon> idWeaponMap = await metadataService.GetIdToWeaponMapAsync().ConfigureAwait(false);

        Dictionary<string, Avatar> nameAvatarMap = await metadataService.GetNameToAvatarMapAsync().ConfigureAwait(false);
        Dictionary<string, Weapon> nameWeaponMap = await metadataService.GetNameToWeaponMapAsync().ConfigureAwait(false);

        List<GachaEvent> gachaevents = await metadataService.GetGachaEventsAsync().ConfigureAwait(false);
        List<HistoryWishBuilder> historyWishBuilders = gachaevents.Select(g => new HistoryWishBuilder(g, nameAvatarMap, nameWeaponMap)).ToList();

        SettingEntry? entry = await appDbContext.Settings
            .SingleOrDefaultAsync(e => e.Key == SettingEntry.IsEmptyHistoryWishVisible)
            .ConfigureAwait(false);

        if (entry == null)
        {
            entry = new(SettingEntry.IsEmptyHistoryWishVisible, true.ToString());
            appDbContext.Settings.Add(entry);
            await appDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        bool isEmptyHistoryWishVisible = bool.Parse(entry.Value!);

        IOrderedEnumerable<GachaItem> orderedItems = items.OrderBy(i => i.Id);
        return await Task.Run(() => CreateCore(orderedItems, historyWishBuilders, idAvatarMap, idWeaponMap, isEmptyHistoryWishVisible)).ConfigureAwait(false);
    }

    private static GachaStatistics CreateCore(
        IOrderedEnumerable<GachaItem> items,
        List<HistoryWishBuilder> historyWishBuilders,
        Dictionary<int, Avatar> avatarMap,
        Dictionary<int, Weapon> weaponMap,
        bool isEmptyHistoryWishVisible)
    {
        TypedWishSummaryBuilder permanentWishBuilder = new("奔行世间", TypedWishSummaryBuilder.PermanentWish, 90, 10);
        TypedWishSummaryBuilder avatarWishBuilder = new("角色活动", TypedWishSummaryBuilder.AvatarEventWish, 90, 10);
        TypedWishSummaryBuilder weaponWishBuilder = new("神铸赋形", TypedWishSummaryBuilder.WeaponEventWish, 80, 10);

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
            // TODO: improve performance.
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
                        isUp = targetHistoryWishBuilder?.IncreaseOrangeAvatar(avatar) ?? false;
                        break;
                    case ItemQuality.QUALITY_PURPLE:
                        purpleAvatarCounter.Increase(avatar);
                        targetHistoryWishBuilder?.IncreasePurpleAvatar(avatar);
                        break;
                }

                permanentWishBuilder.Track(item, avatar, isUp);
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
                        isUp = targetHistoryWishBuilder?.IncreaseOrangeWeapon(weapon) ?? false;
                        orangeWeaponCounter.Increase(weapon);
                        break;
                    case ItemQuality.QUALITY_PURPLE:
                        targetHistoryWishBuilder?.IncreasePurpleWeapon(weapon);
                        purpleWeaponCounter.Increase(weapon);
                        break;
                    case ItemQuality.QUALITY_BLUE:
                        targetHistoryWishBuilder?.IncreaseBlueWeapon(weapon);
                        blueWeaponCounter.Increase(weapon);
                        break;
                }

                permanentWishBuilder.Track(item, weapon, isUp);
                avatarWishBuilder.Track(item, weapon, isUp);
                weaponWishBuilder.Track(item, weapon, isUp);
            }
            else
            {
                // ItemId place not correct.
                Must.NeverHappen();
            }
        }

        return new()
        {
            // history
            HistoryWishes = historyWishBuilders
                .Where(b => isEmptyHistoryWishVisible || (!b.IsEmpty))
                .OrderByDescending(builder => builder.From)
                .ThenBy(builder => builder.ConfigType, new GachaConfigTypeComparar())
                .Select(builder => builder.ToHistoryWish()).ToList(),

            // avatars
            OrangeAvatars = orangeAvatarCounter.ToStatisticsList(),
            PurpleAvatars = purpleAvatarCounter.ToStatisticsList(),

            // weapons
            OrangeWeapons = orangeWeaponCounter.ToStatisticsList(),
            PurpleWeapons = purpleWeaponCounter.ToStatisticsList(),
            BlueWeapons = blueWeaponCounter.ToStatisticsList(),

            // typed wish summary
            PermanentWish = permanentWishBuilder.ToTypedWishSummary(),
            AvatarWish = avatarWishBuilder.ToTypedWishSummary(),
            WeaponWish = weaponWishBuilder.ToTypedWishSummary(),
        };
    }
}
