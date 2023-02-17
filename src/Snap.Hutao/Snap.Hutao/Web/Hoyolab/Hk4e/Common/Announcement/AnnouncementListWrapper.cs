// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

/// <summary>
/// 公告列表
/// </summary>
[HighQuality]
internal sealed class AnnouncementListWrapper : ListWrapper<Announcement>
{
    /// <summary>
    /// 类型Id
    /// </summary>
    [JsonPropertyName("type_id")]
    public int TypeId { get; set; }

    /// <summary>
    /// 类型标签
    /// </summary>
    [JsonPropertyName("type_label")]
    public string TypeLabel { get; set; } = default!;
}
