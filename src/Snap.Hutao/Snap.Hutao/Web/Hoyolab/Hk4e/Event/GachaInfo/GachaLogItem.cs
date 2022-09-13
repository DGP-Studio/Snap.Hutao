// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using MiniExcelLibs.Attributes;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

/// <summary>
/// 祈愿记录物品
/// </summary>
public class GachaLogItem
{
    /// <summary>
    /// 玩家Uid
    /// </summary>
    [ExcelColumn(Name = "uid")]
    [JsonPropertyName("uid")]
    public string Uid { get; set; } = default!;

    /// <summary>
    /// 祈愿类型
    /// </summary>
    [ExcelColumn(Name = "gacha_type")]
    [JsonPropertyName("gacha_type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public GachaConfigType GachaType { get; set; } = default!;

    /// <summary>
    /// 总为 <see cref="string.Empty"/>
    /// </summary>
    [ExcelColumn(Name = "item_id")]
    [Obsolete("API clear this property")]
    [JsonPropertyName("item_id")]
    public string ItemId { get; set; } = default!;

    /// <summary>
    /// 个数 一般为 1
    /// </summary>
    [ExcelColumn(Name = "count")]
    [JsonPropertyName("count")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Count { get; set; }

    /// <summary>
    /// 时间
    /// </summary>
    [ExcelColumn(Name = "time", Format = "yyyy-MM-dd HH:mm:ss")]
    [JsonPropertyName("time")]
    [JsonConverter(typeof(Core.Json.Converter.DateTimeOffsetConverter))]
    public DateTimeOffset Time { get; set; }

    /// <summary>
    /// 物品名称
    /// </summary>
    [ExcelColumn(Name = "name")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 语言
    /// </summary>
    [ExcelColumn(Name = "lang")]
    [JsonPropertyName("lang")]
    public string Language { get; set; } = default!;

    /// <summary>
    /// 物品类型
    /// </summary>
    [ExcelColumn(Name = "item_type")]
    [JsonPropertyName("item_type")]
    public string ItemType { get; set; } = default!;

    /// <summary>
    /// 物品稀有等级
    /// </summary>
    [ExcelColumn(Name = "rank_type")]
    [JsonPropertyName("rank_type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ItemQuality Rank { get; set; } = default!;

    /// <summary>
    /// Id
    /// </summary>
    [ExcelColumn(Name = "id")]
    [JsonPropertyName("id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long Id { get; set; } = default!;
}