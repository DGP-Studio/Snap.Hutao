// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

namespace Snap.Hutao.Service.Announcement;

internal interface IAnnouncementService
{
    ValueTask<AnnouncementWrapper?> GetAnnouncementWrapperAsync(string languageCode, Region region, CancellationToken token = default);
}