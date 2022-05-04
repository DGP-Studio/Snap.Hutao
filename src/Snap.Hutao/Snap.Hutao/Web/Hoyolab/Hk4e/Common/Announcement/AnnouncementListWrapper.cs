// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Newtonsoft.Json;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

/// <summary>
/// 公告列表
/// </summary>
public class AnnouncementListWrapper : ListWrapper<Announcement>
{
    /// <summary>
    /// 类型Id
    /// </summary>
    [JsonProperty("type_id")]
    public int TypeId { get; set; }

    /// <summary>
    /// 类型标签
    /// </summary>
    [JsonProperty("type_label")]
    public string? TypeLabel { get; set; }
}
