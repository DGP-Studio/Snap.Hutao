// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint;

internal interface IHomaServiceEndpoints : IHomaRootAccess
{
    public string Announcement(string locale)
    {
        return $"{Root}/Announcement/List?locale={locale}";
    }

    public string AnnouncementUpload()
    {
        return $"{Root}/Service/Announcement/Upload";
    }

    public string GachaLogCompensation(int days)
    {
        return $"{Root}/Service/GachaLog/Compensation?days={days}";
    }

    public string GachaLogDesignation(string userName, int days)
    {
        return $"{Root}/Service/GachaLog/Designation?userName={userName}&days={days}";
    }
}