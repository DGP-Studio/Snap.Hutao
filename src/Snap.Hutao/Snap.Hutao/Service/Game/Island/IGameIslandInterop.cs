// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Launching;
using Snap.Hutao.Service.Game.Launching.Context;

namespace Snap.Hutao.Service.Game.Island;

internal interface IGameIslandInterop
{
    ValueTask BeforeAsync(BeforeLaunchExecutionContext context, CancellationToken token = default);

    ValueTask WaitForExitAsync(LaunchExecutionContext context, CancellationToken token = default);
}