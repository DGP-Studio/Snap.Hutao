// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.RegularExpressions;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

internal static class AnnouncementRegex
{
    public static readonly Regex VersionUpdateTitleRegex = new(SH.WebAnnouncementMatchVersionUpdateTitle, RegexOptions.Compiled);

    public static readonly Regex VersionUpdateTimeRegex = new(SH.WebAnnouncementMatchVersionUpdateTime, RegexOptions.Compiled);

    public static readonly Regex TransientActivityTimeRegex = new(SH.WebAnnouncementMatchTransientActivityTime, RegexOptions.Compiled);

    public static readonly Regex PersistentActivityTimeRegex = new(SH.WebAnnouncementMatchPersistentActivityTime, RegexOptions.Compiled);

    public static readonly Regex PermanentActivityTimeRegex = new(SH.WebAnnouncementMatchPermanentActivityTime, RegexOptions.Compiled);
}