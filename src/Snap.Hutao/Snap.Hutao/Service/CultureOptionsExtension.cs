// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;

namespace Snap.Hutao.Service;

internal static class CultureOptionsExtension
{
    public static bool LanguageCodeFitsCurrentLocale(this CultureOptions options, string? languageCode)
    {
        if (string.IsNullOrEmpty(languageCode))
        {
            return false;
        }

        // We want to make sure code fits in 1 of 15 metadata locales
        CultureInfo cultureInfo = CultureInfo.GetCultureInfo(languageCode);
        return GetLocaleName(cultureInfo) == options.LocaleName;
    }

    public static string GetLanguageCodeForDocumentationSearch(this CultureOptions options)
    {
        return LocaleNames.GetLanguageCodeForDocumentationSearchFromLocaleName(options.LocaleName);
    }

    internal static string GetLocaleName(CultureInfo cultureInfo)
    {
        while (true)
        {
            if (LocaleNames.TryGetLocaleNameFromLanguageName(cultureInfo.Name, out string? localeName))
            {
                return localeName;
            }

            cultureInfo = cultureInfo.Parent;
        }
    }
}