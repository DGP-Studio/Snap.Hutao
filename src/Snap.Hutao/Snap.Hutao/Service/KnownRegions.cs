// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Web.Hoyolab;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Snap.Hutao.Service;

internal static class KnownRegions
{
    public static ImmutableArray<NameValue<Region>> Value
    {
        get
        {
            Debug.Assert(XamlApplicationLifetime.CultureInfoInitialized);
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