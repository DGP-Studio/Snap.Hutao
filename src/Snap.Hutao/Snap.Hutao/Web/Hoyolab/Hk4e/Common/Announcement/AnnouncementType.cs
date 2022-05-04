// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Newtonsoft.Json;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

/// <summary>
/// 公告类型
/// </summary>
public class AnnouncementType
{
    /// <summary>
    /// Id
    /// </summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 国际化名称
    /// </summary>
    [JsonProperty("mi18n_name")]
    public string? MI18NName { get; set; }
}