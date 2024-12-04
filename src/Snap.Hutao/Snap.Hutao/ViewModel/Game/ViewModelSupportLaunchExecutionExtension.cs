// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.PathAbstraction;

namespace Snap.Hutao.ViewModel.Game;

internal static class ViewModelSupportLaunchExecutionExtension
{
    public static void SetGamePathEntriesAndSelectedGamePathEntry(this IViewModelSupportLaunchExecution viewModel, IRestrictedGamePathAccess access)
    {
        viewModel.SetGamePathEntriesAndSelectedGamePathEntry(access.PerformGamePathEntrySynchronization(out GamePathEntry? selected), selected);
    }
}