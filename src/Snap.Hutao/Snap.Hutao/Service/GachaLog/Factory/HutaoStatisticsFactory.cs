// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Hutao.GachaLog;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.GachaLog.Factory;

internal sealed class HutaoStatisticsFactory
{
    private readonly HutaoStatisticsFactoryMetadataContext context;
    private readonly GachaEvent avatarEvent;
    private readonly GachaEvent avatarEvent2;
    private readonly GachaEvent weaponEvent;
    private readonly GachaEvent? chronicledEvent;

    public HutaoStatisticsFactory(in HutaoStatisticsFactoryMetadataContext context)
    {
        this.context = context;

        // when in new verion
        // due to lack of newer metadata
        // this can crash
        DateTimeOffset now = DateTimeOffset.UtcNow;
        avatarEvent = context.GachaEvents.Single(g => g.From < now && g.To > now && g.Type == GachaType.ActivityAvatar);
        avatarEvent2 = context.GachaEvents.Single(g => g.From < now && g.To > now && g.Type == GachaType.SpecialActivityAvatar);
        weaponEvent = context.GachaEvents.Single(g => g.From < now && g.To > now && g.Type == GachaType.ActivityWeapon);
        chronicledEvent = context.GachaEvents.SingleOrDefault(g => g.From < now && g.To > now && g.Type == GachaType.ActivityCity);
    }

    public HutaoStatistics Create(GachaEventStatistics raw)
    {
        return new()
        {
            AvatarEvent = CreateWishSummary(avatarEvent, raw.AvatarEvent),
            AvatarEvent2 = CreateWishSummary(avatarEvent2, raw.AvatarEvent2),
            WeaponEvent = CreateWishSummary(weaponEvent, raw.WeaponEvent),
            Chronicled = chronicledEvent is null ? null : CreateWishSummary(chronicledEvent, raw.Chronicled),
        };
    }

    private HutaoWishSummary CreateWishSummary(GachaEvent gachaEvent, List<ItemCount> items)
    {
        List<StatisticsItem> upItems = [];
        List<StatisticsItem> orangeItems = [];
        List<StatisticsItem> purpleItems = [];
        List<StatisticsItem> blueItems = [];

        foreach (ref readonly ItemCount item in CollectionsMarshal.AsSpan(items))
        {
            IStatisticsItemSource source = item.Item.StringLength() switch
            {
                8U => context.GetAvatar(item.Item),
                5U => context.GetWeapon(item.Item),
                _ => throw HutaoException.GachaStatisticsInvalidItemId(item.Item),
            };
            StatisticsItem statisticsItem = source.ToStatisticsItem(unchecked((int)item.Count));

            // Put UP items to a separate list
            if (gachaEvent.UpOrangeList.Contains(item.Item) || gachaEvent.UpPurpleList.Contains(item.Item))
            {
                upItems.Add(statisticsItem);
            }
            else
            {
                List<StatisticsItem> list = statisticsItem.Quality switch
                {
                    QualityType.QUALITY_ORANGE => orangeItems,
                    QualityType.QUALITY_PURPLE => purpleItems,
                    QualityType.QUALITY_BLUE => blueItems,
                    _ => throw Must.NeverHappen("意外的物品等级"),
                };

                list.Add(statisticsItem);
            }
        }

        return new()
        {
            Event = gachaEvent,
            UpItems = [.. upItems.OrderByDescending(i => i.Quality).ThenByDescending(i => i.Count)],
            OrangeItems = [.. orangeItems.OrderByDescending(i => i.Count)],
            PurpleItems = [.. purpleItems.OrderByDescending(i => i.Count)],
            BlueItems = [.. blueItems.OrderByDescending(i => i.Count)],
        };
    }
}