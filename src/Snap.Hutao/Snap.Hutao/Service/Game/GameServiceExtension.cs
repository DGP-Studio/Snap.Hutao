// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game.Scheme;

namespace Snap.Hutao.Service.Game;

internal static class GameServiceExtension
{
    public static GameAccount? DetectCurrentGameAccount(this IGameService gameService, LaunchScheme scheme)
    {
        return gameService.DetectCurrentGameAccount(scheme.GetSchemeType());
    }

    public static ValueTask<GameAccount?> DetectGameAccountAsync(this IGameService gameService, LaunchScheme scheme)
    {
        return gameService.DetectGameAccountAsync(scheme.GetSchemeType());
    }
}