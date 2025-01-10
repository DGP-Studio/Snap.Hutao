// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Web.Hoyolab;
using System.Collections.Immutable;

namespace Snap.Hutao.Service;

internal static class KnownRegions
{
    public static ImmutableArray<NameValue<Region>> Value
    {
        get
        {
            // This array must be lazy-loaded because it depends on the localization.
            return !field.IsDefault ? field : field =
            [
                new(SH.WebHoyolabRegionCNGF01, Region.CNGF01),
                new(SH.WebHoyolabRegionCNQD01, Region.CNQD01),
                new(SH.WebHoyolabRegionOSUSA, Region.OSUSA),
                new(SH.WebHoyolabRegionOSEURO, Region.OSEURO),
                new(SH.WebHoyolabRegionOSASIA, Region.OSASIA),
                new(SH.WebHoyolabRegionOSCHT, Region.OSCHT),
            ];
        }
    }
}