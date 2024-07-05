// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.RegularExpressions;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

internal static partial class AnnouncementRegex
{
    /// <inheritdoc cref="SH.WebAnnouncementMatchVersionUpdateTitle"/>
    public static readonly Regex VersionUpdateTitleRegex = new(SHRegex.WebAnnouncementMatchVersionUpdateTitle, RegexOptions.Compiled);

    /// <inheritdoc cref="SH.WebAnnouncementMatchVersionUpdateTime"/>
    public static readonly Regex VersionUpdateTimeRegex = new(SHRegex.WebAnnouncementMatchVersionUpdateTime, RegexOptions.Compiled);

    /// <inheritdoc cref="SH.WebAnnouncementMatchVersionUpdatePreviewTitle"/>
    public static readonly Regex VersionUpdatePreviewTitleRegex = new(SHRegex.WebAnnouncementMatchVersionUpdatePreviewTitle, RegexOptions.Compiled);

    /// <inheritdoc cref="SH.WebAnnouncementMatchVersionUpdatePreviewTime"/>
    public static readonly Regex VersionUpdatePreviewTimeRegex = new(SHRegex.WebAnnouncementMatchVersionUpdatePreviewTime, RegexOptions.Compiled);

    /// <inheritdoc cref="SH.WebAnnouncementMatchTransientActivityTime"/>
    public static readonly Regex TransientActivityAfterUpdateTimeRegex = new(SHRegex.WebAnnouncementMatchTransientActivityTime, RegexOptions.Compiled);

    /// <inheritdoc cref="SH.WebAnnouncementMatchPersistentActivityTime"/>
    public static readonly Regex PersistentActivityAfterUpdateTimeRegex = new(SHRegex.WebAnnouncementMatchPersistentActivityTime, RegexOptions.Compiled);

    /// <inheritdoc cref="SH.WebAnnouncementMatchPermanentActivityTime"/>
    public static readonly Regex PermanentActivityAfterUpdateTimeRegex = new(SHRegex.WebAnnouncementMatchPermanentActivityTime, RegexOptions.Compiled);

    [GeneratedRegex("&lt;t class=\"t_(?:gl|lc)\".*?&gt;(.*?)&lt;/t&gt;", RegexOptions.Multiline)]
    public static partial Regex XmlTimeTagRegex();
}