// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Service;

internal static class AppOptionsExtension
{
    public static NameValue<Region>? GetCurrentRegionForSelectionOrDefault(this AppOptions appOptions)
    {
        return appOptions.LazyRegions.Value.SingleOrDefault(c => c.Value.Value == appOptions.Region.Value);
    }

    public static NameValue<TimeSpan>? GetCalendarServerTimeZoneOffsetForSelectionOrDefault(this AppOptions appOptions)
    {
        return appOptions.LazyCalendarServerTimeZoneOffsets.Value.SingleOrDefault(c => c.Value == appOptions.CalendarServerTimeZoneOffset);
    }
}