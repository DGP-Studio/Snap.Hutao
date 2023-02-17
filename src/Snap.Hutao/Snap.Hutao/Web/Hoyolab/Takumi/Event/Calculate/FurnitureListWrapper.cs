// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

/// <summary>
/// 家具列表包装器
/// </summary>
[HighQuality]
internal sealed class FurnitureListWrapper : ListWrapper<Item>
{
    /// <summary>
    /// 无法计算的物品
    /// </summary>
    [JsonPropertyName("not_calc_list")]
    public List<Item>? NotCalculateList { get; set; }
}