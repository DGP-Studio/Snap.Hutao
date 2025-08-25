// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Intrinsic;

[ExtendedEnum]
internal enum AssociationType
{
    ASSOC_TYPE_NONE,

    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeMondstadt))]
    ASSOC_TYPE_MONDSTADT,

    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeLiyue))]
    ASSOC_TYPE_LIYUE,
    ASSOC_TYPE_MAINACTOR,

    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeFatui))]
    ASSOC_TYPE_FATUI,

    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeInazuma))]
    ASSOC_TYPE_INAZUMA,

    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeRanger))]
    ASSOC_TYPE_RANGER,

    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeSumeru))]
    ASSOC_TYPE_SUMERU,

    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeFontaine))]
    ASSOC_TYPE_FONTAINE,

    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeNatlan))]
    ASSOC_TYPE_NATLAN,

    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeSnezhnaya))]
    ASSOC_TYPE_SNEZHNAYA,

    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeOmniScourge))]
    ASSOC_TYPE_OMNI_SCOURGE,

    [LocalizationKey(nameof(SH.ModelIntrinsicAssociationTypeNodkrai))]
    ASSOC_TYPE_NODKRAI,
}