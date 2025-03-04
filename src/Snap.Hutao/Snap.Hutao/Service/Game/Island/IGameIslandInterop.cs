// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Island;

internal interface IGameIslandInterop
{
    ValueTask WaitForExitAsync(bool resume = false, CancellationToken token = default);

    ValueTask<bool> PrepareAsync(bool resume = false, CancellationToken token = default);
}