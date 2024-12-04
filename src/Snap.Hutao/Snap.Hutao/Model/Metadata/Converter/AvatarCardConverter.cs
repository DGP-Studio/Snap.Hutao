// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Data.Converter;
using Snap.Hutao.Web.Endpoint.Hutao;

namespace Snap.Hutao.Model.Metadata.Converter;

internal sealed partial class AvatarCardConverter : ValueConverter<string, Uri>, IIconNameToUriConverter
{
    private const string CostumeCard = "UI_AvatarIcon_Costume_Card.png";
    private static readonly Uri UIAvatarIconCostumeCard = StaticResourcesEndpoints.StaticRaw("AvatarCard", CostumeCard).ToUri();

    public static Uri IconNameToUri(string name)
    {
        return string.IsNullOrEmpty(name)
            ? UIAvatarIconCostumeCard
            : StaticResourcesEndpoints.StaticRaw("AvatarCard", $"{name}_Card.png").ToUri();
    }

    public override Uri Convert(string from)
    {
        return IconNameToUri(from);
    }
}