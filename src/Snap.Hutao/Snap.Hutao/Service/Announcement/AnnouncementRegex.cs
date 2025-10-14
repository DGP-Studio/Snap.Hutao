// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.RegularExpressions;

namespace Snap.Hutao.Service.Announcement;

internal static partial class AnnouncementRegex
{
    [GeneratedRegex("&lt;t class=\"t_(?:gl|lc)\".*?&gt;(?:<span .*?>)?(.*?)(?:</span>)?&lt;/t&gt;", RegexOptions.Multiline)]
    public static partial Regex XmlTimeTagRegex { get; }
}