// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab;

internal readonly partial struct Region
{
    public Region(string value, bool? isOversea = default)
    {
        Must.Argument(HoyolabRegex.RegionRegex().IsMatch(value), SH.WebHoyolabInvalidRegion);
        DisplayName = EvaluateDisplayName(value);
        Value = value;
        IsOversea = isOversea ?? EvaluateIsOversea(value.AsSpan()[0]);
    }

    public readonly string DisplayName { get; }

    public readonly string Value { get; }

    public readonly bool IsOversea { get; }

    public static implicit operator Region(string value)
    {
        return FromRegion(value);
    }

    public static Region FromRegion(string value)
    {
        return new(value);
    }

    public static Region FromUid(string uid)
    {
        return uid.AsSpan()[0] switch
        {
            // CN
            >= '1' and <= '4' => new("cn_gf01"), // 国服
            '5' => new("cn_qd01"),               // 渠道

            // OS
            '6' => new("os_usa"),                // 美服
            '7' => new("os_euro"),               // 欧服
            '8' => new("os_asia"),               // 亚服
            '9' => new("os_cht"),                // 台服
            _ => throw Must.NeverHappen(),
        };
    }

    private static string EvaluateDisplayName(string value)
    {
        return value switch
        {
            "cn_gf01" => SH.WebHoyolabRegionCnGf01,
            "cn_qd01" => SH.WebHoyolabRegionCnQd01,
            "os_usa" => SH.WebHoyolabRegionOsUsa,
            "os_euro" => SH.WebHoyolabRegionOsEuro,
            "os_asia" => SH.WebHoyolabRegionOsAsia,
            "os_cht" => SH.WebHoyolabRegionOsCht,
            _ => throw Must.NeverHappen(),
        };
    }

    private static bool EvaluateIsOversea(in char first)
    {
        return first switch
        {
            'c' => false,
            'o' => true,
            _ => throw Must.NeverHappen(),
        };
    }
}
