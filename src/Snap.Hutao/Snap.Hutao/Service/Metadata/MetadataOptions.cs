// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using System.Globalization;
using System.IO;

namespace Snap.Hutao.Service.Metadata;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class MetadataOptions
{
    private readonly CultureOptions cultureOptions;
    private readonly RuntimeOptions runtimeOptions;

    private string? fallbackDataFolder;
    private string? localizedDataFolder;

    public string FallbackDataFolder
    {
        get
        {
            if (fallbackDataFolder is null)
            {
                fallbackDataFolder = Path.Combine(runtimeOptions.DataFolder, "Metadata", LocaleNames.CHS);
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
                localizedDataFolder = Path.Combine(runtimeOptions.DataFolder, "Metadata", cultureOptions.LocaleName);
                Directory.CreateDirectory(localizedDataFolder);
            }

            return localizedDataFolder;
        }
    }

    public string GetLocalizedLocalFile(string fileNameWithExtension)
    {
        return Path.Combine(LocalizedDataFolder, fileNameWithExtension);
    }

    public string GetLocalizedRemoteFile(string fileNameWithExtension)
    {
        return Web.HutaoEndpoints.Metadata(cultureOptions.LocaleName, fileNameWithExtension);
    }
}