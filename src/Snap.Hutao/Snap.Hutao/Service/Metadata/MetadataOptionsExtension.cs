// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;
using System.IO;

namespace Snap.Hutao.Service.Metadata;

internal static class MetadataOptionsExtension
{
    public static string GetLocalizedLocalFile(this MetadataOptions options, string fileNameWithExtension)
    {
        return Path.Combine(options.LocalizedDataFolder, fileNameWithExtension);
    }

    public static string GetLocalizedRemoteFile(this MetadataOptions options, string fileNameWithExtension)
    {
        return Web.HutaoEndpoints.Metadata(options.LocaleName, fileNameWithExtension);
    }

    public static bool LanguageCodeFitsCurrentLocale(this MetadataOptions options, string? languageCode)
    {
        if (string.IsNullOrEmpty(languageCode))
        {
            return false;
        }

        // We want to make sure code fits in 1 of 15 metadata locales
        CultureInfo cultureInfo = CultureInfo.GetCultureInfo(languageCode);
        return GetLocaleName(cultureInfo) == options.LocaleName;
    }

    internal static string GetLocaleName(CultureInfo cultureInfo)
    {
        while (true)
        {
            if (LocaleNames.TryGetLocaleNameFromLanguageName(cultureInfo.Name, out string? localeName))
            {
                return localeName;
            }
            else
            {
                cultureInfo = cultureInfo.Parent;
            }
        }
    }
}