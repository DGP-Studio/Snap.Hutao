// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.Guide;

[ExtendedEnum]
internal enum StaticResourceQuality
{
    [LocalizationKey(nameof(SH.ViewModelGuideStaticResourceQualityRaw))]
    Original,

    [LocalizationKey(nameof(SH.ViewModelGuideStaticResourceQualityHigh))]
    High,
}