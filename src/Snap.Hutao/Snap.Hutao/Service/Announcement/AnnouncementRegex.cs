// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.RegularExpressions;

namespace Snap.Hutao.Service.Announcement;

internal static partial class AnnouncementRegex
{
    /// <inheritdoc cref="SHRegex.ServiceAnnouncementMatchValidDescriptions"/>
    public static readonly Regex ValidDescriptionsRegex = new(SHRegex.ServiceAnnouncementMatchValidDescriptions, RegexOptions.Compiled);

    /// <inheritdoc cref="SHRegex.ServiceAnnouncementMatchVersionUpdateTitle"/>
    public static readonly Regex VersionUpdateTitleRegex = new(SHRegex.ServiceAnnouncementMatchVersionUpdateTitle, RegexOptions.Compiled);

    /// <inheritdoc cref="SHRegex.ServiceAnnouncementMatchVersionUpdatePreviewTitle"/>
    public static readonly Regex VersionUpdatePreviewTitleRegex = new(SHRegex.ServiceAnnouncementMatchVersionUpdatePreviewTitle, RegexOptions.Compiled);

    /// <inheritdoc cref="SHRegex.ServiceAnnouncementMatchVersionUpdatePreviewTime"/>
    public static readonly Regex VersionUpdatePreviewTimeRegex = new(SHRegex.ServiceAnnouncementMatchVersionUpdatePreviewTime, RegexOptions.Compiled);

    [GeneratedRegex("&lt;t class=\"t_(?:gl|lc)\".*?&gt;(?:<span .*?>)?(.*?)(?:</span>)?&lt;/t&gt;", RegexOptions.Multiline)]
    public static partial Regex XmlTimeTagRegex { get; }
}