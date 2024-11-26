// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.PathAbstraction;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game;

internal interface IRestrictedGamePathAccess
{
    string GamePath { get; set; }

    ImmutableArray<GamePathEntry> GamePathEntries { get; set; }

    AsyncReaderWriterLock GamePathLock { get; }
}