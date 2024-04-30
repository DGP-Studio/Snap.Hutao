// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.RegularExpressions;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

internal static partial class AnnouncementRegex
{
    /// <inheritdoc cref="SH.WebAnnouncementMatchVersionUpdateTitle"/>
    public static readonly Regex VersionUpdateTitleRegex = new(SH.WebAnnouncementMatchVersionUpdateTitle, RegexOptions.Compiled);

    /// <inheritdoc cref="SH.WebAnnouncementMatchVersionUpdateTime"/>
    public static readonly Regex VersionUpdateTimeRegex = new(SH.WebAnnouncementMatchVersionUpdateTime, RegexOptions.Compiled);

    /// <inheritdoc cref="SH.WebAnnouncementMatchVersionUpdatePreviewTitle"/>
    public static readonly Regex VersionUpdatePreviewTitleRegex = new(SH.WebAnnouncementMatchVersionUpdatePreviewTitle, RegexOptions.Compiled);

    /// <inheritdoc cref="SH.WebAnnouncementMatchVersionUpdatePreviewTime"/>
    public static readonly Regex VersionUpdatePreviewTimeRegex = new(SH.WebAnnouncementMatchVersionUpdatePreviewTime, RegexOptions.Compiled);

    /// <inheritdoc cref="SH.WebAnnouncementMatchTransientActivityTime"/>
    public static readonly Regex TransientActivityAfterUpdateTimeRegex = new(SH.WebAnnouncementMatchTransientActivityTime, RegexOptions.Compiled);

    /// <inheritdoc cref="SH.WebAnnouncementMatchPersistentActivityTime"/>
    public static readonly Regex PersistentActivityAfterUpdateTimeRegex = new(SH.WebAnnouncementMatchPersistentActivityTime, RegexOptions.Compiled);

    /// <inheritdoc cref="SH.WebAnnouncementMatchPermanentActivityTime"/>
    public static readonly Regex PermanentActivityAfterUpdateTimeRegex = new(SH.WebAnnouncementMatchPermanentActivityTime, RegexOptions.Compiled);

    [GeneratedRegex("&lt;t class=\"t_(?:gl|lc)\".*?&gt;(.*?)&lt;/t&gt;", RegexOptions.Multiline)]
    public static partial Regex XmlTimeTagRegex();
}