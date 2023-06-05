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
    private readonly HutaoOptions hutaoOptions;

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
        get => localeName ??= GetLocaleName();
    }

    /// <inheritdoc/>
    public MetadataOptions Value { get => this; }

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
        return Web.HutaoEndpoints.RawGithubUserContentMetadataFile(LocaleName, fileNameWithExtension);
    }

    private string GetLocaleName()
    {
        CultureInfo cultureInfo = appOptions.CurrentCulture;

        while (true)
        {
            if (cultureInfo == CultureInfo.InvariantCulture)
            {
                // Fallback to Chinese.
                return "CHS";
            }

            switch (cultureInfo.Name)
            {
                case "de": return "DE";         // German
                case "en": return "EN";         // English
                case "es": return "ES";         // Spanish
                case "fr": return "FR";         // French
                case "id": return "ID";         // Indonesian
                case "it": return "IT";         // Italian
                case "ja": return "JP";         // Japanese
                case "ko": return "KR";         // Korean
                case "pt": return "PT";         // Portuguese
                case "ru": return "RU";         // Russian
                case "th": return "TH";         // Thai
                case "tr": return "TR";         // Turkish
                case "vi": return "TR";         // Vietnamese
                case "zh-CHS": return "CHS";    // Chinese (Simplified) Legacy
                case "zh-CHT": return "CHT";    // Chinese (Traditional) Legacy
                default: cultureInfo = cultureInfo.Parent; break;
            }
        }
    }
}
