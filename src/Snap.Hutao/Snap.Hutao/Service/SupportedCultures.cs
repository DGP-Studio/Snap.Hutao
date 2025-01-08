// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using System.Collections.Immutable;
using System.Globalization;

namespace Snap.Hutao.Service;

[SuppressMessage("", "SA1500")]
[SuppressMessage("", "SA1513")]
internal static class SupportedCultures
{
    public static ImmutableArray<NameCultureInfoValue> Values
    {
        get;
    } =
    [
        ToNameValue(CultureInfo.GetCultureInfo("zh-Hans")),
        ToNameValue(CultureInfo.GetCultureInfo("zh-Hant")),
        /*ToNameValue(CultureInfo.GetCultureInfo("de")),*/
        ToNameValue(CultureInfo.GetCultureInfo("en")),
        /*ToNameValue(CultureInfo.GetCultureInfo("es")),*/
        ToNameValue(CultureInfo.GetCultureInfo("fr")),
        ToNameValue(CultureInfo.GetCultureInfo("id")),
        /*ToNameValue(CultureInfo.GetCultureInfo("it")),*/
        ToNameValue(CultureInfo.GetCultureInfo("ja")),
        ToNameValue(CultureInfo.GetCultureInfo("ko")),
        ToNameValue(CultureInfo.GetCultureInfo("pt")),
        ToNameValue(CultureInfo.GetCultureInfo("ru")),
        /*ToNameValue(CultureInfo.GetCultureInfo("th")),*/
        /*ToNameValue(CultureInfo.GetCultureInfo("tr")),*/
        ToNameValue(CultureInfo.GetCultureInfo("vi")),
    ];

    private static NameCultureInfoValue ToNameValue(CultureInfo info)
    {
        return new(info.NativeName, info);
    }
}