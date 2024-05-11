// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.LifeCycle;

/// <summary>
/// 激活
/// </summary>
internal interface IActivation
{
    void Activate(HutaoActivationArguments args);

    void PostInitialization();
}