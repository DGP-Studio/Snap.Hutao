// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.


namespace Snap.Hutao.Web.Hoyolab;

internal readonly partial struct Region
{
    public readonly string DisplayName;

    public readonly string Value;

    public readonly bool IsOversea;

    public Region(string value, bool? isOversea = default)
    {
        Must.Argument(HoyolabRegex.RegionRegex().IsMatch(value), SH.WebHoyolabInvalidRegion);
        DisplayName = EvaluateDisplayName(value);
        Value = value;
        IsOversea = isOversea ?? EvaluateIsOversea(value.AsSpan()[0]);
    }

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

    public override string ToString()
    {
        return Value;
    }

    private static string EvaluateDisplayName(string value)
    {
        return value switch
        {
            "cn_gf01" => SH.WebHoyolabRegionCNGF01,
            "cn_qd01" => SH.WebHoyolabRegionCNQD01,
            "os_usa" => SH.WebHoyolabRegionOSUSA,
            "os_euro" => SH.WebHoyolabRegionOSEURO,
            "os_asia" => SH.WebHoyolabRegionOSASIA,
            "os_cht" => SH.WebHoyolabRegionOSCHT,
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
