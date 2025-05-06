// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppNotifications;

namespace Snap.Hutao.Core.LifeCycle;

internal interface IAppActivation
{
    void RedirectedActivate(HutaoActivationArguments args);

    void NotificationInvoked(AppNotificationManager manager, AppNotificationActivatedEventArgs args);

    void ActivateAndInitialize(HutaoActivationArguments args);
}