// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Service.Metadata;

/// <summary>
/// 本地化名称
/// </summary>
internal static class LocaleNames
{
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
    public const string CHS = "CHS";    // Chinese (Simplified)
    public const string CHT = "CHT";    // Chinese (Traditional)

    public static readonly ImmutableDictionary<string, string> LanguageNameLocaleNameMap = new Dictionary<string, string>()
    {
        ["de"] = DE,
        ["en"] = EN,
        ["es"] = ES,
        ["fr"] = FR,
        ["id"] = ID,
        ["it"] = IT,
        ["ja"] = JP,
        ["ko"] = KR,
        ["pt"] = PT,
        ["ru"] = RU,
        ["th"] = TH,
        ["tr"] = TR,
        ["vi"] = VI,
        ["zh-Hans"] = CHS,
        ["zh-Hant"] = CHT,
        [string.Empty] = CHS, // Fallback to Chinese.
    }.ToImmutableDictionary();

    public static readonly ImmutableDictionary<string, string> LocaleNameLanguageCodeMap = new Dictionary<string, string>()
    {
        [DE] = "de-de",
        [EN] = "en-us",
        [ES] = "es-es",
        [FR] = "fr-fr",
        [ID] = "id-id",
        [IT] = "it-it",
        [JP] = "ja-jp",
        [KR] = "ko-kr",
        [PT] = "pt-pt",
        [RU] = "ru-ru",
        [TH] = "th-th",
        [TR] = "tr-tr",
        [VI] = "vi-vn",
        [CHS] = "zh-cn",
        [CHT] = "zh-tw",
    }.ToImmutableDictionary();
}