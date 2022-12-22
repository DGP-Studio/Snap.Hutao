// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Bridge.Model;

/// <summary>
/// 导航页面参数
/// </summary>
public class PushPagePayload
{
    /// <summary>
    /// 页面Url
    /// </summary>
    [JsonPropertyName("page")]
    public string Page { get; set; } = default!;
}