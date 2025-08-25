// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Intrinsic;

[ExtendedEnum]
internal enum BodyType
{
    BODY_NONE,

    [LocalizationKey(nameof(SH.ModelIntrinsicBodyTypeBoy))]
    BODY_BOY,

    [LocalizationKey(nameof(SH.ModelIntrinsicBodyTypeGirl))]
    BODY_GIRL,

    [LocalizationKey(nameof(SH.ModelIntrinsicBodyTypeLady))]
    BODY_LADY,

    [LocalizationKey(nameof(SH.ModelIntrinsicBodyTypeMale))]
    BODY_MALE,

    [LocalizationKey(nameof(SH.ModelIntrinsicBodyTypeLoli))]
    BODY_LOLI,
}