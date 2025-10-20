// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;

namespace Snap.Hutao.Service;

internal static class LocaleNames
{
    public const string CHS = "CHS";    // Chinese (Simplified)
    public const string CHT = "CHT";    // Chinese (Traditional)
    public const string DE = "DE";      // German
    public const string EN = "EN";      // English
    public const string ES = "ES";      // Spanish
    public const string FR = "FR";      // French
    public const string ID = "ID";      // Indonesian
    public const string IT = "IT";      // Italian
    public const string JP = "JP";      // Japanese
    public const string KR = "KR";      // Korean
    public const string PT = "PT";      // Portuguese
    public const string RU = "RU";      // Russian
    public const string TH = "TH";      // Thai
    public const string TR = "TR";      // Turkish
    public const string VI = "VI";      // Vietnamese

    public static bool TryGetLocaleNameFromLanguageName(string languageName, [NotNullWhen(true)] out string? localeName)
    {
        localeName = languageName switch
        {
            "de" => DE,
            "en" => EN,
            "es" => ES,
            "fr" => FR,
            "id" => ID,
            "it" => IT,
            "ja" => JP,
            "ko" => KR,
            "pt" => PT,
            "ru" => RU,
            "th" => TH,
            "tr" => TR,
            "vi" => VI,
            "zh-Hans" => CHS,
            "zh-Hant" => CHT,
            "" => CHS, // Fallback to Chinese.
            _ => string.Empty,
        };

        return !string.IsNullOrEmpty(localeName);
    }

    public static bool TryGetLanguageCodeFromLocaleName(string localeName, [NotNullWhen(true)] out string? languageCode)
    {
        languageCode = localeName switch
        {
            DE => "de-de",
            EN => "en-us",
            ES => "es-es",
            FR => "fr-fr",
            ID => "id-id",
            IT => "it-it",
            JP => "ja-jp",
            KR => "ko-kr",
            PT => "pt-pt",
            RU => "ru-ru",
            TH => "th-th",
            TR => "tr-tr",
            VI => "vi-vn",
            CHS => "zh-cn",
            CHT => "zh-tw",
            _ => string.Empty,
        };

        return !string.IsNullOrEmpty(languageCode);
    }

    public static string GetLanguageCodeForDocumentationSearchFromLocaleName(string localeName)
    {
        return localeName switch
        {
            ID => "id-id",
            JP => "ja-jp",
            RU => "ru-ru",
            CHS => "zh-cn",
            _ => "en-us",
        };
    }

    public static string GetLocaleName(CultureInfo cultureInfo)
    {
        while (true)
        {
            if (TryGetLocaleNameFromLanguageName(cultureInfo.Name, out string? localeName))
            {
                return localeName;
            }

            cultureInfo = cultureInfo.Parent;
        }
    }

    public static bool LanguageCodeFitsCurrentLocale(string? languageCode, string localeName)
    {
        if (string.IsNullOrEmpty(languageCode))
        {
            return false;
        }

        // We want to make sure code fits in 1 of 15 metadata locales
        CultureInfo cultureInfo = CultureInfo.GetCultureInfo(languageCode);
        return string.Equals(GetLocaleName(cultureInfo), localeName, StringComparison.Ordinal);
    }
}