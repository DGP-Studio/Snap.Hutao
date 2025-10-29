// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Web.Endpoint.Hutao;
using System.IO;

namespace Snap.Hutao.Service.Metadata;

[GeneratedConstructor]
[Service(ServiceLifetime.Singleton)]
internal sealed partial class MetadataOptions
{
    private readonly IHutaoEndpointsFactory hutaoEndpointsFactory;
    private readonly CultureOptions cultureOptions;

    [field: MaybeNull]
    public string FallbackDataFolder
    {
        get
        {
            if (field is null)
            {
                field = Path.Combine(HutaoRuntime.DataDirectory, "Metadata", LocaleNames.CHS);
                Directory.CreateDirectory(field);
            }

            return field;
        }
    }

    [field: MaybeNull]
    public string LocalizedDataFolder
    {
        get
        {
            if (field is null)
            {
                field = Path.Combine(HutaoRuntime.DataDirectory, "Metadata", cultureOptions.LocaleName);
                Directory.CreateDirectory(field);
            }

            return field;
        }
    }

    public string GetTemplateEndpoint()
    {
        return hutaoEndpointsFactory.Create().MetadataTemplate();
    }

    public string GetLocalizedLocalPath(string fileNameWithExtension)
    {
        return Path.Combine(LocalizedDataFolder, fileNameWithExtension);
    }

    public string GetLocalizedRemoteFile(MetadataTemplate? templateInfo, string fileNameWithExtension)
    {
        return templateInfo is { Template: { } template }
            ? hutaoEndpointsFactory.Create().Metadata(template, cultureOptions.LocaleName, fileNameWithExtension)
            : hutaoEndpointsFactory.Create().Metadata(cultureOptions.LocaleName, fileNameWithExtension);
    }
}