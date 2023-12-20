// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Service;

internal static class KnownRegions
{
    private static readonly Region RegionCNGF01 = new("cn_gf01");
    private static readonly Region RegionCNQD01 = new("cn_qd01");
    private static readonly Region RegionOSUSA = new("os_usa");
    private static readonly Region RegionOSEURO = new("os_euro");
    private static readonly Region RegionOSASIA = new("os_asia");
    private static readonly Region RegionOSCHT = new("os_cht");

    public static List<Region> Get()
    {
        return
        [
            RegionCNGF01,
            RegionCNQD01,
            RegionOSUSA,
            RegionOSEURO,
            RegionOSASIA,
            RegionOSCHT,
        ];
    }
}