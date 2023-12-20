// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Service;

internal static class KnownRegions
{
    private static readonly Region RegionCnGf01 = new("cn_gf01");
    private static readonly Region RegionCnQd01 = new("cn_qd01");
    private static readonly Region RegionOsUsa = new("os_usa");
    private static readonly Region RegionOsEuro = new("os_euro");
    private static readonly Region RegionOsAsia = new("os_asia");
    private static readonly Region RegionOsCht = new("os_cht");

    public static List<Region> Get()
    {
        return
        [
            RegionCnGf01,
            RegionCnQd01,
            RegionOsUsa,
            RegionOsEuro,
            RegionOsAsia,
            RegionOsCht,
        ];
    }
}