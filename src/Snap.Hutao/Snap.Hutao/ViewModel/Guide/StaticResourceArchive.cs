// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.Guide;

[ExtendedEnum]
internal enum StaticResourceArchive
{
    [LocalizationKey(nameof(SH.ViewGuideStepStaticResourceSettingMinimumOff))]
    Full,

    [LocalizationKey(nameof(SH.ViewGuideStepStaticResourceSettingMinimumOn))]
    Minimum,
}