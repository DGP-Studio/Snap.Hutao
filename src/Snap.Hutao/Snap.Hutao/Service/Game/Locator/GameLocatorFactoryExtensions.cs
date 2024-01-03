// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Locator;

internal static class GameLocatorFactoryExtensions
{
    public static ValueTask<ValueResult<bool, string>> LocateAsync(this IGameLocatorFactory factory, GameLocationSource source)
    {
        return factory.Create(source).LocateGamePathAsync();
    }
}