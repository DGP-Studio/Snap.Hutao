// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Service.Game.PathAbstraction;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game.FileSystem;

internal interface IRestrictedGamePathAccess
{
    [Obsolete]
    IObservableProperty<string> GamePath { get; }

    IObservableProperty<GamePathEntry?> GamePathEntry { get; }

    IObservableProperty<ImmutableArray<GamePathEntry>> GamePathEntries { get; }

    AsyncReaderWriterLock GamePathLock { get; }
}