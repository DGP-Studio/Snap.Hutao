// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.LifeCycle;

internal interface IAppActivation
{
    void Activate(HutaoActivationArguments args);

    void PostInitialization();
}

internal interface IAppActivationActionHandlersAccess
{
    ValueTask HandleLaunchGameActionAsync(string? uid = null);
}