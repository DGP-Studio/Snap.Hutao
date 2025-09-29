// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Text.Json.Annotation;
using Snap.Hutao.Core.Text.Json.Converter;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Model.InterChange.GachaLog;

// ReSharper disable once InconsistentNaming
internal sealed class Hk4eItem : IJsonOnDeserialized
{
    // ReSharper disable once InconsistentNaming
    [JsonPropertyName("uigf_gacha_type")]
    [JsonEnumHandling(JsonEnumHandling.NumberString)]
    public required GachaType UIGFGachaType { get; init; }

    [JsonPropertyName("gacha_type")]
    [JsonEnumHandling(JsonEnumHandling.NumberString)]
    public required GachaType GachaType { get; init; }

    [JsonPropertyName("item_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public required uint ItemId { get; init; }

    [JsonPropertyName("time")]
    [JsonConverter(typeof(SimpleDateTimeConverter))]
    public required DateTime Time { get; init; }

    [JsonPropertyName("id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public required long Id { get; init; }

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

    public void OnDeserialized()
    {
        if (!Enum.IsDefined(UIGFGachaType))
        {
            throw new JsonException($"Unsupported UIGFGachaType: {UIGFGachaType}");
        }

        if (!Enum.IsDefined(GachaType))
        {
            throw new JsonException($"Unsupported GachaType: {GachaType}");
        }
    }
}