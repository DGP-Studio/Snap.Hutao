// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal sealed class UserInfoWrapper
{
    [JsonPropertyName("user_info")]
    public UserInformation UserInfo { get; set; } = default!;
}