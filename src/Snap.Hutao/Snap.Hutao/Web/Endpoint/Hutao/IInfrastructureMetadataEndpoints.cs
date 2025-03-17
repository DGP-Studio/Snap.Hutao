// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;

namespace Snap.Hutao.Web.Endpoint.Hutao;

internal interface IInfrastructureMetadataEndpoints : IInfrastructureRootAccess
{
    public string Metadata(string locale, string fileName)
    {
        return $"{Root}/metadata/Genshin/{locale}/{fileName}";
    }

    public string Metadata(string template, string locale, string fileName)
    {
        return string.Format(CultureInfo.CurrentCulture, template, $"Genshin/{locale}/{fileName}");
    }

    public string MetadataTemplate()
    {
        return $"{Root}/metadata/template";
    }
}