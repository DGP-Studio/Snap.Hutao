// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// 用户信息包装器
/// </summary>
public class UserInfoWrapper
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [JsonPropertyName("user_info")]
    public UserInformation UserInfo { get; set; } = default!;
}