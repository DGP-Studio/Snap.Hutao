// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game.Package;
using Snap.Hutao.Service.Game.PathAbstraction;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.UI.Xaml.View.Dialog;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Game;

internal interface IViewModelSupportLaunchExecution
{
    LaunchGameShared Shared { get; }

    LaunchScheme? SelectedScheme { get; }

    GameAccount? SelectedGameAccount { get; }

    void SetGamePathEntriesAndSelectedGamePathEntry(ImmutableArray<GamePathEntry> gamePathEntries, GamePathEntry? selectedEntry)
    {
        // Do nothing
    }
}

internal interface IViewModelSupportLaunchExecution2
{
    LaunchScheme? TargetScheme { get; }

    LaunchScheme CurrentScheme { get; }

    GameAccount? GameAccount { get; }

    ValueTask<BlockDeferralWithProgress<PackageConvertStatus>> CreateConvertBlockDeferralAsync()
    {
        return BlockDeferralWithProgress<PackageConvertStatus>.CreateAsync<LaunchGamePackageConvertDialog>(default, static (state, dialog) => dialog.State = state);
    }
}