// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Web.Endpoint.Hutao;
using System.IO;

namespace Snap.Hutao.Service.Metadata;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
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
                field = Path.Combine(HutaoRuntime.DataFolder, "Metadata", LocaleNames.CHS);
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
                field = Path.Combine(HutaoRuntime.DataFolder, "Metadata", cultureOptions.LocaleName);
                Directory.CreateDirectory(field);
            }

            return field;
        }
    }

    public string GetLocalizedLocalPath(string fileNameWithExtension)
    {
        return Path.Combine(LocalizedDataFolder, fileNameWithExtension);
    }

    public string GetLocalizedRemoteFile(string fileNameWithExtension)
    {
        return hutaoEndpointsFactory.Create().Metadata(cultureOptions.LocaleName, fileNameWithExtension);
    }
}