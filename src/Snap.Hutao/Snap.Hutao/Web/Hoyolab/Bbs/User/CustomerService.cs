// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

/// <summary>
/// 售后服务
/// </summary>
public class CustomerService
{
    /// <summary>
    /// 是否为客服
    /// </summary>
    [JsonPropertyName("is_customer_service_staff")]
    public bool IsCustomerServiceStaff { get; set; }

    /// <summary>
    /// 负责的游戏Id
    /// </summary>
    [JsonPropertyName("game_id")]
    public int GameId { get; set; }
}
