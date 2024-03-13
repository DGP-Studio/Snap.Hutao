// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Unlocker;

/// <summary>
/// 游戏帧率解锁器
/// </summary>
[HighQuality]
internal interface IGameFpsUnlocker
{
    ValueTask PostUnlockAsync(CancellationToken token = default);

    ValueTask UnlockAsync(CancellationToken token = default);
}