// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.ViewModel.Game;
using Snap.Hutao.ViewModel.User;

namespace Snap.Hutao.Service.Game.Launching;

[ConstructorGenerated]
internal sealed partial class LaunchExecutionContext : IDisposable
{
    private IGameFileSystem? gameFileSystem;

    public LaunchExecutionContext(IServiceProvider serviceProvider, IViewModelSupportLaunchExecution viewModel, UserAndUid? userAndUid)
        : this(serviceProvider)
    {
        ViewModel = viewModel;

        LaunchScheme? currentScheme = viewModel.Shared.GetCurrentLaunchSchemeFromConfigFile();
        ArgumentNullException.ThrowIfNull(currentScheme);
        CurrentScheme = currentScheme;

        LaunchScheme? targetScheme = viewModel.SelectedScheme;
        ArgumentNullException.ThrowIfNull(targetScheme);
        TargetScheme = targetScheme;

        Account = viewModel.SelectedGameAccount;
        UserAndUid = userAndUid;
    }

    public LaunchExecutionResult Result { get; } = new();

    public partial IServiceProvider ServiceProvider { get; }

    public partial ITaskContext TaskContext { get; }

    public partial ILogger<LaunchExecutionContext> Logger { get; }

    public partial LaunchOptions Options { get; }

    public IViewModelSupportLaunchExecution ViewModel { get; }

    public LaunchScheme CurrentScheme { get; private set; }

    public LaunchScheme TargetScheme { get; private set; }

    public GameAccount? Account { get; private set; }

    public string? AuthTicket { get; set; }

    public UserAndUid? UserAndUid { get; private set; }

    public bool ChannelOptionsChanged { get; set; }

    public IProgress<LaunchStatus> Progress { get; set; } = default!;

    public System.Diagnostics.Process Process { get; set; } = default!;

    public bool TryGetGameFileSystem([NotNullWhen(true)] out IGameFileSystem? gameFileSystem)
    {
        if (this.gameFileSystem is not null)
        {
            gameFileSystem = this.gameFileSystem;
            return true;
        }

        if (!Options.TryGetGameFileSystem(out gameFileSystem))
        {
            Result.Kind = LaunchExecutionResultKind.NoActiveGamePath;
            Result.ErrorMessage = SH.ServiceGameLaunchExecutionGamePathNotValid;
            return false;
        }

        this.gameFileSystem = gameFileSystem;
        return true;
    }

    public void PerformGamePathEntrySynchronization()
    {
        // Invalidate game file system
        gameFileSystem?.Dispose();
        gameFileSystem = null;

        ViewModel.SetGamePathEntriesAndSelectedGamePathEntry(Options);
    }

    public void UpdateGamePath(string gamePath)
    {
        // Invalidate game file system
        gameFileSystem?.Dispose();
        gameFileSystem = null;

        Options.GamePath = gamePath;
        PerformGamePathEntrySynchronization();
    }

    public void Dispose()
    {
        gameFileSystem?.Dispose();
    }
}