// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game.PathAbstraction;
using Snap.Hutao.Service.Notification;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Game;

internal interface IViewModelSupportLaunchExecution
{
    LaunchGameShared Shared { get; }

    GameAccount? SelectedGameAccount { get; }

    void SetGamePathEntriesAndSelectedGamePathEntry(ImmutableList<GamePathEntry> gamePathEntries, GamePathEntry? selectedEntry)
    {
        // Do nothing
    }
}