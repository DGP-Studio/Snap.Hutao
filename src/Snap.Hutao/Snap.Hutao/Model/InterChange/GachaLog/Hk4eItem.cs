// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Core.Json.Annotation;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Model.InterChange.GachaLog;

internal sealed class Hk4eItem
{
    [JsonPropertyName("uigf_gacha_type")]
    [JsonEnum(JsonEnumSerializeType.NumberString)]
    public required GachaType UIGFGachaType { get; set; }

    [JsonPropertyName("gacha_type")]
    [JsonEnum(JsonEnumSerializeType.NumberString)]
    public required GachaType GachaType { get; set; }

    [JsonPropertyName("item_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public required uint ItemId { get; set; }

    [JsonPropertyName("time")]
    [JsonConverter(typeof(Core.Json.Converter.DateTimeConverter))]
    public required DateTime Time { get; set; }

    [JsonPropertyName("id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public required long Id { get; set; } = default!;

    public static Hk4eItem From(GachaItem item)
    {
        return new()
        {
            UIGFGachaType = item.QueryType,
            GachaType = item.GachaType,
            ItemId = item.ItemId,
            Time = item.Time.UtcDateTime,
            Id = item.Id,
        };
    }
}