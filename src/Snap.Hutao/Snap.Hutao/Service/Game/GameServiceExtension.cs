// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game.Scheme;

namespace Snap.Hutao.Service.Game;

internal static class GameServiceExtension
{
    public static GameAccount? DetectCurrentGameAccountNoThrow(this IGameService gameService, LaunchScheme scheme)
    {
        try
        {
            return gameService.DetectCurrentGameAccount(scheme.GetSchemeType());
        }
        catch
        {
            return default;
        }
    }

    public static ValueTask<GameAccount?> DetectGameAccountAsync(this IGameService gameService, LaunchScheme scheme, Func<string, Task<ValueResult<bool, string?>>> providerNameCallback)
    {
        return gameService.DetectGameAccountAsync(scheme.GetSchemeType(), providerNameCallback);
    }
}