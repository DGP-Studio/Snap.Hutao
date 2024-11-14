// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

namespace Snap.Hutao.Service.Announcement;

/// <summary>
/// 公告服务
/// </summary>
[HighQuality]
internal interface IAnnouncementService
{
    Task<AnnouncementWrapper?> GetAnnouncementWrapperAsync(string languageCode, Region region, CancellationToken token = default);
}