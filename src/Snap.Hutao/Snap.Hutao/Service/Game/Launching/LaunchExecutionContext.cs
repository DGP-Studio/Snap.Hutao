// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.ViewModel.Game;
using Snap.Hutao.ViewModel.User;

namespace Snap.Hutao.Service.Game.Launching;

internal sealed partial class LaunchExecutionContext : AbstractLaunchExecutionContext
{
    public LaunchExecutionContext(IServiceProvider serviceProvider, IViewModelSupportLaunchExecution viewModel, UserAndUid? userAndUid)
        : base(serviceProvider)
    {
        ViewModel = new(viewModel);

        LaunchScheme? currentScheme = viewModel.Shared.GetCurrentLaunchSchemeFromConfigFile();
        ArgumentNullException.ThrowIfNull(currentScheme);
        CurrentScheme = currentScheme;
        TargetScheme = viewModel.SelectedScheme ?? currentScheme;

        Account = viewModel.SelectedGameAccount;
        UserAndUid = userAndUid;
    }

    public WeakReference<IViewModelSupportLaunchExecution> ViewModel { get; }

    public LaunchScheme CurrentScheme { get; }

    public LaunchScheme TargetScheme { get; }

    public GameAccount? Account { get; }

    public UserAndUid? UserAndUid { get; }

    public void PerformGamePathEntrySynchronization()
    {
        lock (SyncRoot)
        {
            CheckDisposedAndDispose();

            if (ViewModel.TryGetTarget(out IViewModelSupportLaunchExecution? viewModel))
            {
                viewModel.SetGamePathEntriesAndSelectedGamePathEntry(Options);
            }
        }
    }

    public void UpdateGamePath(string gamePath)
    {
        lock (SyncRoot)
        {
            CheckDisposedAndDispose();

            Options.GamePath = gamePath;
            PerformGamePathEntrySynchronization();
        }
    }
}