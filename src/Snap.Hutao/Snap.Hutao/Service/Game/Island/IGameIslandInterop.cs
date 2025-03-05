// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Island;

internal interface IGameIslandInterop
{
    ValueTask WaitForExitAsync(CancellationToken token = default);

    ValueTask<bool> PrepareAsync(CancellationToken token = default);
}