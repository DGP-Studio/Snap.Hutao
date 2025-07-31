// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.UI.Xaml.Data.Converter;
using Snap.Hutao.Web.Endpoint.Hutao;

namespace Snap.Hutao.Model.Metadata.Converter;

internal sealed partial class AssociationTypeIconConverter : ValueConverter<AssociationType, Uri?>
{
    public static Uri? AssociationTypeToIconUri(AssociationType type)
    {
        string? association = type switch
        {
            AssociationType.ASSOC_TYPE_MONDSTADT => "Mengde",
            AssociationType.ASSOC_TYPE_LIYUE => "Liyue",
            AssociationType.ASSOC_TYPE_FATUI => default,
            AssociationType.ASSOC_TYPE_INAZUMA => "Inazuma",
            AssociationType.ASSOC_TYPE_RANGER => default,
            AssociationType.ASSOC_TYPE_SUMERU => "Sumeru",
            AssociationType.ASSOC_TYPE_FONTAINE => "Fontaine",
            AssociationType.ASSOC_TYPE_NATLAN => "Natlan",
            AssociationType.ASSOC_TYPE_SNEZHNAYA => default,
            AssociationType.ASSOC_TYPE_OMNI_SCOURGE => default,
            AssociationType.ASSOC_TYPE_NODKRAI => "Nodkrai",
            _ => throw HutaoException.NotSupported(),
        };

        return association is null
            ? default
            : StaticResourcesEndpoints.StaticRaw("ChapterIcon", $"UI_ChapterIcon_{association}.png").ToUri();
    }

    public override Uri? Convert(AssociationType from)
    {
        return AssociationTypeToIconUri(from);
    }
}