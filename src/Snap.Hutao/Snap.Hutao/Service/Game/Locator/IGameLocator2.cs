// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game.Locator;

internal interface IGameLocator2
{
    ValueTask<ImmutableArray<string>> LocateMultipleGamePathAsync();
}