// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using System.IO;

namespace Snap.Hutao.Service.Metadata;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class MetadataOptions
{
    private readonly AppOptions appOptions;
    private readonly RuntimeOptions hutaoOptions;

    private string? localeName;
    private string? fallbackDataFolder;
    private string? localizedDataFolder;

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

    public string LocaleName
    {
        get => localeName ??= MetadataOptionsExtension.GetLocaleName(appOptions.CurrentCulture);
    }

    public string LanguageCode
    {
        get
        {
            if (LocaleNames.TryGetLanguageCodeFromLocaleName(LocaleName, out string? languageCode))
            {
                return languageCode;
            }

            throw new KeyNotFoundException($"Invalid localeName: '{LocaleName}'");
        }
    }
}