// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;

namespace Snap.Hutao.Web.Hutao;

internal static class LocalizableResponseExtension
{
    public static string? GetLocalizationMessageOrDefault(this ILocalizableResponse localizableResponse)
    {
        string? key = localizableResponse.LocalizationKey;
        if (string.IsNullOrEmpty(key))
        {
            return default;
        }

        return SH.ResourceManager.GetString(key, CultureInfo.CurrentCulture);
    }

    public static string GetLocalizationMessage(this ILocalizableResponse localizableResponse)
    {
        string? key = localizableResponse.LocalizationKey;
        if (string.IsNullOrEmpty(key))
        {
            return string.Empty;
        }

        return SH.ResourceManager.GetString(key, CultureInfo.CurrentCulture) ?? string.Empty;
    }
}