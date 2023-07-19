// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Core.Json.Annotation;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Model.InterChange.GachaLog;

/// <summary>
/// UIGF物品
/// </summary>
[HighQuality]
internal sealed class UIGFItem : GachaLogItem, IMappingFrom<UIGFItem, GachaItem, INameQuality>
{
    /// <summary>
    /// 额外祈愿映射
    /// </summary>
    [JsonPropertyName("uigf_gacha_type")]
    [JsonEnum(JsonSerializeType.NumberString)]
    public GachaConfigType UIGFGachaType { get; set; } = default!;

    public static UIGFItem From(GachaItem item, INameQuality nameQuality)
    {
        return new()
        {
            GachaType = item.GachaType,
            ItemId = $"{item.ItemId:D}",
            Count = 1,
            Time = item.Time,
            Name = nameQuality.Name,
            ItemType = GetItemTypeStringByItemId(item.ItemId),
            Rank = nameQuality.Quality,
            Id = item.Id,
            UIGFGachaType = item.QueryType,
        };
    }

    private static string GetItemTypeStringByItemId(uint itemId)
    {
        return itemId.Place() switch
        {
            8U => SH.ModelInterchangeUIGFItemTypeAvatar,
            5U => SH.ModelInterchangeUIGFItemTypeWeapon,
            _ => SH.ModelInterchangeUIGFItemTypeUnknown,
        };
    }
}