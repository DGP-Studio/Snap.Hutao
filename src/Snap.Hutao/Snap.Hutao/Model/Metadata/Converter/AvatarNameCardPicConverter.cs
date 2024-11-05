﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Data.Converter;
using Snap.Hutao.Web.Endpoint.Hutao;

namespace Snap.Hutao.Model.Metadata.Converter;

internal sealed partial class AvatarNameCardPicConverter : ValueConverter<string, Uri>, IIconNameToUriConverter
{
    public static Uri IconNameToUri(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return default!;
        }

        return StaticResourcesEndpoints.StaticRaw("NameCardPic", $"{name}_P.png").ToUri();
    }

    public override Uri Convert(string from)
    {
        return IconNameToUri(from);
    }
}