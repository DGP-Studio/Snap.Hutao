// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Extension;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

/// <summary>
/// 家具列表包装器
/// </summary>
public class FurnitureListWrapper : ListWrapper<Item>
{
    /// <summary>
    /// 无法计算的物品
    /// </summary>
    [JsonPropertyName("not_calc_list")]
    public List<Item>? NotCalculateList { get; set; }
}