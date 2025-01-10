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
        ToNameValue(CultureInfo.GetCultureInfo("zh-Hans"), LocalizationSource.Hutao),
        ToNameValue(CultureInfo.GetCultureInfo("zh-Hant"), LocalizationSource.Crowdin),
        /*ToNameValue(CultureInfo.GetCultureInfo("de")),*/
        ToNameValue(CultureInfo.GetCultureInfo("en"), LocalizationSource.Hutao),
        /*ToNameValue(CultureInfo.GetCultureInfo("es")),*/
        ToNameValue(CultureInfo.GetCultureInfo("fr"), LocalizationSource.Gemini),
        ToNameValue(CultureInfo.GetCultureInfo("id"), LocalizationSource.Gemini),
        /*ToNameValue(CultureInfo.GetCultureInfo("it")),*/
        ToNameValue(CultureInfo.GetCultureInfo("ja"), LocalizationSource.Crowdin),
        ToNameValue(CultureInfo.GetCultureInfo("ko"), LocalizationSource.Crowdin | LocalizationSource.Gemini),
        ToNameValue(CultureInfo.GetCultureInfo("pt"), LocalizationSource.Gemini),
        ToNameValue(CultureInfo.GetCultureInfo("ru"), LocalizationSource.Gemini),
        /*ToNameValue(CultureInfo.GetCultureInfo("th")),*/
        /*ToNameValue(CultureInfo.GetCultureInfo("tr")),*/
        ToNameValue(CultureInfo.GetCultureInfo("vi"), LocalizationSource.Gemini),
    ];

    private static NameCultureInfoValue ToNameValue(CultureInfo info, LocalizationSource source)
    {
        return new(info.NativeName, info, source);
    }
}