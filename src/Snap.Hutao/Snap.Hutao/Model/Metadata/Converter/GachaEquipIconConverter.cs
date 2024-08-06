// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Data.Converter;
using Snap.Hutao.Web.Endpoint;

namespace Snap.Hutao.Model.Metadata.Converter;

internal sealed class GachaEquipIconConverter : ValueConverter<string, Uri>, IIconNameToUriConverter
{
    public static Uri IconNameToUri(string name)
    {
        string icon = name["UI_".Length..];
        return new Uri(StaticResourcesEndpoints.StaticRaw("GachaEquipIcon", $"UI_Gacha_{icon}.png"));
    }

    public override Uri Convert(string from)
    {
        return IconNameToUri(from);
    }
}