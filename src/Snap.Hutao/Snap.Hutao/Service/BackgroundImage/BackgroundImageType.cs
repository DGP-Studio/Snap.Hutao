// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.BackgroundImage;

[ExtendedEnum]
internal enum BackgroundImageType
{
    [LocalizationKey(nameof(SH.ServiceBackgroundImageTypeNone))]
    None,

    [LocalizationKey(nameof(SH.ServiceBackgroundImageTypeLocalFolder))]
    LocalFolder,

    [LocalizationKey(nameof(SH.ServiceBackgroundImageTypeBing))]
    HutaoBing,

    [LocalizationKey(nameof(SH.ServiceBackgroundImageTypeDaily))]
    HutaoDaily,

    [LocalizationKey(nameof(SH.ServiceBackgroundImageTypeLauncher))]
    HutaoOfficialLauncher,
}