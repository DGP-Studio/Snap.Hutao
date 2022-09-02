// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

/// <summary>
/// 公告包装器
/// </summary>
public class AnnouncementWrapper : ListWrapper<AnnouncementListWrapper>
{
    /// <summary>
    /// 总数
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>
    /// 类型列表
    /// </summary>
    [JsonPropertyName("type_list")]
    public List<AnnouncementType> TypeList { get; set; } = default!;

    /// <summary>
    /// 提醒
    /// </summary>
    [JsonPropertyName("alert")]
    public bool Alert { get; set; }

    /// <summary>
    /// 提醒Id
    /// </summary>
    [JsonPropertyName("alert_id")]
    public int AlertId { get; set; }

    /// <summary>
    /// 时区
    /// </summary>
    [JsonPropertyName("timezone")]
    public int TimeZone { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    [JsonPropertyName("t")]
    public string TimeStamp { get; set; } = default!;
}