// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service;

[Localization]
internal enum LastWindowCloseBehavior
{
    [LocalizationKey("ServiceCloseButtonBehaviorTypeMinimize")]
    EnsureNotifyIconCreated,

    [LocalizationKey("ServiceCloseButtonBehaviorTypeExit")]
    ExitApplication,
}