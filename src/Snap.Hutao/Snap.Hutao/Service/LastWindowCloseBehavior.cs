// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service;

[ExtendedEnum]
internal enum LastWindowCloseBehavior
{
    [LocalizationKey(nameof(SH.ServiceCloseButtonBehaviorTypeMinimize))]
    EnsureNotifyIconCreated,

    [LocalizationKey(nameof(SH.ServiceCloseButtonBehaviorTypeExit))]
    ExitApplication,
}