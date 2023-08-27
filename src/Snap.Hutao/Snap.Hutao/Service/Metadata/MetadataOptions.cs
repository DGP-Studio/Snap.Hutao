// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Options;
using Snap.Hutao.Core;
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
            if (fallbackDataFolder is null)
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
            if (localizedDataFolder is null)
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
        if (string.IsNullOrEmpty(languageCode))
        {
            return false;
        }

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