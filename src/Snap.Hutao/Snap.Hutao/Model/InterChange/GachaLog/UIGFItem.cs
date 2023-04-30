// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Annotation;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Model.InterChange.GachaLog;

/// <summary>
/// UIGF物品
/// </summary>
[HighQuality]
internal sealed class UIGFItem : GachaLogItem
{
    /// <summary>
    /// 额外祈愿映射
    /// </summary>
    [JsonPropertyName("uigf_gacha_type")]
    [JsonEnum(JsonSerializeType.NumberString)]
    public GachaConfigType UIGFGachaType { get; set; } = default!;
}