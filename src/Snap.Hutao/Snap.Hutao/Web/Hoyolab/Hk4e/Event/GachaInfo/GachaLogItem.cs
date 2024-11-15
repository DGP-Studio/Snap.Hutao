﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Annotation;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

internal class GachaLogItem
{
    [JsonPropertyName("uid")]
    public string Uid { get; set; } = default!;

    [JsonPropertyName("gacha_type")]
    [JsonEnum(JsonEnumSerializeType.NumberString)]
    public GachaType GachaType { get; set; } = default!;

    [JsonPropertyName("item_id")]
    public string ItemId { get; set; } = default!;

    [JsonPropertyName("count")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public int? Count { get; set; }

    [JsonPropertyName("time")]
    [JsonConverter(typeof(Core.Json.Converter.DateTimeOffsetConverter))]
    public DateTimeOffset Time { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("lang")]
    public string Language { get; set; } = default!;

    [JsonPropertyName("item_type")]
    public string ItemType { get; set; } = default!;

    [JsonPropertyName("rank_type")]
    [JsonEnum(JsonEnumSerializeType.NumberString)]
    public QualityType RankType { get; set; } = default!;

    [JsonPropertyName("id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long Id { get; set; } = default!;
}