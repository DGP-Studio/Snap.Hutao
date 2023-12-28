// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game.Scheme;

namespace Snap.Hutao.Service.Game;

internal static class GameServiceFacadeExtension
{
    public static GameAccount? DetectCurrentGameAccount(this IGameServiceFacade gameServiceFacade, LaunchScheme scheme)
    {
        return gameServiceFacade.DetectCurrentGameAccount(scheme.GetSchemeType());
    }

    public static ValueTask<GameAccount?> DetectGameAccountAsync(this IGameServiceFacade gameServiceFacade, LaunchScheme scheme)
    {
        return gameServiceFacade.DetectGameAccountAsync(scheme.GetSchemeType());
    }
}