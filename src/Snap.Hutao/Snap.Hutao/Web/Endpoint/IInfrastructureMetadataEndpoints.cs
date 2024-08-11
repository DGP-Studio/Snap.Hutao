﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint;

internal interface IInfrastructureMetadataEndpoints : IInfrastructureRootAccess
{
    public string Metadata(string locale, string fileName)
    {
        return $"{Root}/metadata/Genshin/{locale}/{fileName}";
    }
}