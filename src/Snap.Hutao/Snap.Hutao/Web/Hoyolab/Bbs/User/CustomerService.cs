// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

internal sealed class CustomerService
{
    [JsonPropertyName("is_customer_service_staff")]
    public bool IsCustomerServiceStaff { get; set; }

    [JsonPropertyName("game_id")]
    public int GameId { get; set; }
}
