// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using System.Globalization;

namespace Snap.Hutao.Service;

internal static class SupportedCultures
{
    private static readonly List<NameValue<CultureInfo>> Cultures =
    [
        ToNameValue(CultureInfo.GetCultureInfo("zh-Hans")),
        ToNameValue(CultureInfo.GetCultureInfo("zh-Hant")),
        /*ToNameValue(CultureInfo.GetCultureInfo("de")),*/
        ToNameValue(CultureInfo.GetCultureInfo("en")),
        /*ToNameValue(CultureInfo.GetCultureInfo("es")),*/
        /*ToNameValue(CultureInfo.GetCultureInfo("fr")),*/
        ToNameValue(CultureInfo.GetCultureInfo("id")),
        /*ToNameValue(CultureInfo.GetCultureInfo("it")),*/
        ToNameValue(CultureInfo.GetCultureInfo("ja")),
        ToNameValue(CultureInfo.GetCultureInfo("ko")),
        ToNameValue(CultureInfo.GetCultureInfo("pt")),
        ToNameValue(CultureInfo.GetCultureInfo("ru")),
        /*ToNameValue(CultureInfo.GetCultureInfo("th")),*/
        /*ToNameValue(CultureInfo.GetCultureInfo("tr")),*/
        /*ToNameValue(CultureInfo.GetCultureInfo("vi")),*/
    ];

    public static List<NameValue<CultureInfo>> Get()
    {
        return Cultures;
    }

    private static NameValue<CultureInfo> ToNameValue(CultureInfo info)
    {
        return new(info.NativeName, info);
    }
}