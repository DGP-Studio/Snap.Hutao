// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Options;
using Snap.Hutao.Core;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;

namespace Snap.Hutao.Service.Metadata;

/// <summary>
/// 元数据选项
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class MetadataOptions : IOptions<MetadataOptions>
{


    private readonly AppOptions appOptions;
    private readonly RuntimeOptions hutaoOptions;

    private string? localeName;
    private string? fallbackDataFolder;
    private string? localizedDataFolder;

    /// <summary>
    /// 中文数据文件夹
    /// </summary>
    public string FallbackDataFolder
    {
        get
        {
            if (fallbackDataFolder == null)
            {
                fallbackDataFolder = Path.Combine(hutaoOptions.DataFolder, "Metadata", "CHS");
                Directory.CreateDirectory(fallbackDataFolder);
            }

            return fallbackDataFolder;
        }
    }

    /// <summary>
    /// 本地化数据文件夹
    /// </summary>
    public string LocalizedDataFolder
    {
        get
        {
            if (localizedDataFolder == null)
            {
                localizedDataFolder = Path.Combine(hutaoOptions.DataFolder, "Metadata", LocaleName);
                Directory.CreateDirectory(localizedDataFolder);
            }

            return localizedDataFolder;
        }
    }

    /// <summary>
    /// 当前使用的元数据本地化名称
    /// </summary>
    public string LocaleName
    {
        get => localeName ??= GetLocaleName(appOptions.CurrentCulture);
    }

    /// <summary>
    /// 当前语言代码
    /// </summary>
    public string LanguageCode
    {
        get => LocaleNames.LocaleNameLanguageCodeMap[LocaleName];
    }

    /// <inheritdoc/>
    public MetadataOptions Value { get => this; }

    /// <summary>
    /// 获取语言名称
    /// </summary>
    /// <param name="cultureInfo">语言信息</param>
    /// <returns>元数据语言名称</returns>
    public static string GetLocaleName(CultureInfo cultureInfo)
    {
        while (true)
        {
            if (LocaleNames.LanguageNameLocaleNameMap.TryGetValue(cultureInfo.Name, out string? localeName))
            {
                return localeName;
            }
            else
            {
                cultureInfo = cultureInfo.Parent;
            }
        }
    }

    /// <summary>
    /// 检查是否为当前语言名称
    /// </summary>
    /// <param name="languageCode">语言代码</param>
    /// <returns>是否为当前语言名称</returns>
    public bool IsCurrentLocale(string languageCode)
    {
        CultureInfo cultureInfo = CultureInfo.GetCultureInfo(languageCode);
        return GetLocaleName(cultureInfo) == LocaleName;
    }

    /// <summary>
    /// 获取本地的本地化元数据文件
    /// </summary>
    /// <param name="fileNameWithExtension">文件名</param>
    /// <returns>本地的本地化元数据文件</returns>
    public string GetLocalizedLocalFile(string fileNameWithExtension)
    {
        return Path.Combine(LocalizedDataFolder, fileNameWithExtension);
    }

    /// <summary>
    /// 获取服务器上的本地化元数据文件
    /// </summary>
    /// <param name="fileNameWithExtension">文件名</param>
    /// <returns>服务器上的本地化元数据文件</returns>
    public string GetLocalizedRemoteFile(string fileNameWithExtension)
    {
#if DEBUG
        return Web.HutaoEndpoints.HutaoMetadata2File(LocaleName, fileNameWithExtension);
#else
        return Web.HutaoEndpoints.HutaoMetadata2File(LocaleName, fileNameWithExtension);
#endif
    }
}

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