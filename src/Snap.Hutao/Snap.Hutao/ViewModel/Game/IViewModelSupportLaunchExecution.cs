// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game.PathAbstraction;
using Snap.Hutao.ViewModel.User;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Game;

internal interface IViewModelSupportLaunchExecution
{
    LaunchGameShared Shared { get; }

    GameAccount? SelectedGameAccount { get; }

    UserAndUid? SelectedUserAndUid { get; }

    void SetGamePathEntriesAndSelectedGamePathEntry(ImmutableList<GamePathEntry> gamePathEntries, GamePathEntry? selectedEntry)
    {
        // Do nothing
    }
}