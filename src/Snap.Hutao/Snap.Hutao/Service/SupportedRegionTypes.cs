// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Service;

internal static class SupportedRegionTypes
{
    private static readonly List<NameValue<RegionType>> Cultures =
    [
        ToNameValue(RegionType.CN_GF01),
        ToNameValue(RegionType.CN_QD01),
        ToNameValue(RegionType.OS_USA),
        ToNameValue(RegionType.OS_EURO),
        ToNameValue(RegionType.OS_ASIA),
        ToNameValue(RegionType.OS_CHT),
    ];

    public static List<NameValue<RegionType>> Get()
    {
        return Cultures;
    }

    private static NameValue<RegionType> ToNameValue(RegionType regionType)
    {
        return new(
            regionType switch
            {
                RegionType.CN_GF01 => SH.ModelIntrinsicRegionTypeCnGf01,
                RegionType.CN_QD01 => SH.ModelIntrinsicRegionTypeCnQd01,
                RegionType.OS_USA => SH.ModelIntrinsicRegionTypeOsUsa,
                RegionType.OS_EURO => SH.ModelIntrinsicRegionTypeOsEuro,
                RegionType.OS_ASIA => SH.ModelIntrinsicRegionTypeOsAsia,
                RegionType.OS_CHT => SH.ModelIntrinsicRegionTypeOsCht,
                _ => SH.ModelIntrinsicRegionTypeCnGf01,
            },
            regionType);
    }
}