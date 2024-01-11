// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.PathAbstraction;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Game;

internal interface IViewModelSupportLaunchExecution
{
    void SetGamePathEntriesAndSelectedGamePathEntry(ImmutableList<GamePathEntry> gamePathEntries, GamePathEntry? selectedEntry);
}