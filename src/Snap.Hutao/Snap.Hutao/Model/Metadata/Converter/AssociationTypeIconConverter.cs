// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Converter;

internal sealed class AssociationTypeIconConverter : ValueConverter<AssociationType, Uri?>
{
    private static readonly Dictionary<string, AssociationType> LocalizedNameToAssociationType = new()
    {
        [SH.ModelIntrinsicAssociationTypeMondstadt] = AssociationType.ASSOC_TYPE_MONDSTADT,
        [SH.ModelIntrinsicAssociationTypeLiyue] = AssociationType.ASSOC_TYPE_LIYUE,
        [SH.ModelIntrinsicAssociationTypeFatui] = AssociationType.ASSOC_TYPE_FATUI,
        [SH.ModelIntrinsicAssociationTypeInazuma] = AssociationType.ASSOC_TYPE_INAZUMA,
        [SH.ModelIntrinsicAssociationTypeRanger] = AssociationType.ASSOC_TYPE_RANGER,
        [SH.ModelIntrinsicAssociationTypeSumeru] = AssociationType.ASSOC_TYPE_SUMERU,
        [SH.ModelIntrinsicAssociationTypeFontaine] = AssociationType.ASSOC_TYPE_FONTAINE,
        [SH.ModelIntrinsicAssociationTypeNatlan] = AssociationType.ASSOC_TYPE_NATLAN,
        [SH.ModelIntrinsicAssociationTypeSnezhnaya] = AssociationType.ASSOC_TYPE_SNEZHNAYA,
    };

    public static Uri? AssociationTypeNameToIconUri(string associationTypeName)
    {
        return AssociationTypeToIconUri(LocalizedNameToAssociationType.GetValueOrDefault(associationTypeName));
    }

    public static Uri? AssociationTypeToIconUri(AssociationType type)
    {
        string? association = type switch
        {
            AssociationType.ASSOC_TYPE_MONDSTADT => "Mengde",
            AssociationType.ASSOC_TYPE_LIYUE => "Liyue",
            AssociationType.ASSOC_TYPE_FATUI => null,
            AssociationType.ASSOC_TYPE_INAZUMA => "Inazuma",
            AssociationType.ASSOC_TYPE_RANGER => null,
            AssociationType.ASSOC_TYPE_SUMERU => "Sumeru",
            AssociationType.ASSOC_TYPE_FONTAINE => "Fontaine",
            AssociationType.ASSOC_TYPE_NATLAN => null,
            AssociationType.ASSOC_TYPE_SNEZHNAYA => null,
            _ => throw Must.NeverHappen(),
        };

        return association is null
            ? null
            : Web.HutaoEndpoints.StaticRaw("ChapterIcon", $"UI_ChapterIcon_{association}.png").ToUri();
    }

    public override Uri? Convert(AssociationType from)
    {
        return AssociationTypeToIconUri(from);
    }
}
