// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

/// <summary>
/// 用户信息
/// </summary>
internal sealed class UserInfo
{
    /// <summary>
    /// 是否为开发者
    /// </summary>
    public bool IsLicensedDeveloper { get; set; }

    /// <summary>
    /// 祈愿记录服务到期时间
    /// </summary>
    public DateTimeOffset GachaLogExpireAt { get; set; }
}