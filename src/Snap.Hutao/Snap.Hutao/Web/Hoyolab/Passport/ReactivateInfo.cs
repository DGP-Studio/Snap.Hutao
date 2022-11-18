// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// 重新激活信息
/// </summary>
public class ReactivateInfo
{
    /// <summary>
    /// 需要重新激活
    /// </summary>
    [JsonPropertyName("required")]
    public bool Required { get; set; }

    /// <summary>
    /// 重新激活凭证
    /// </summary>
    [JsonPropertyName("ticket")]
    public string Ticket { get; set; } = default!;
}