// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game.PathAbstraction;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.ViewModel.Game;
using Snap.Hutao.ViewModel.User;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game.Launching;

[ConstructorGenerated]
internal sealed partial class LaunchExecutionContext : IDisposable
{
    private readonly ILogger<LaunchExecutionContext> logger;

    private IGameFileSystem? gameFileSystem;

    [SuppressMessage("", "SH007")]
    public LaunchExecutionContext(IServiceProvider serviceProvider, IViewModelSupportLaunchExecution viewModel, LaunchScheme? targetScheme, GameAccount? account, UserAndUid? userAndUid)
        : this(serviceProvider)
    {
        ViewModel = viewModel;
        CurrentScheme = viewModel.Shared.GetCurrentLaunchSchemeFromConfigFile()!;
        TargetScheme = targetScheme!;
        Account = account;
        UserAndUid = userAndUid;
    }

    public LaunchExecutionResult Result { get; } = new();

    public CancellationToken CancellationToken { get; set; }

    public partial IServiceProvider ServiceProvider { get; }

    public partial ITaskContext TaskContext { get; }

    public ILogger Logger { get => logger; }

    public partial LaunchOptions Options { get; }

    public IViewModelSupportLaunchExecution ViewModel { get; private set; } = default!;

    public LaunchScheme CurrentScheme { get; private set; } = default!;

    public LaunchScheme TargetScheme { get; private set; } = default!;

    public GameAccount? Account { get; private set; }

    public UserAndUid? UserAndUid { get; private set; }

    public bool ChannelOptionsChanged { get; set; }

    public IProgress<LaunchStatus> Progress { get; set; } = default!;

    public string? AuthTicket { get; set; }

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

    public void UpdateGamePathEntry()
    {
        // Invalidate game file system
        gameFileSystem?.Dispose();
        gameFileSystem = null;

        ImmutableArray<GamePathEntry> gamePathEntries = Options.GetGamePathEntries(out GamePathEntry? selectedEntry);
        ViewModel.SetGamePathEntriesAndSelectedGamePathEntry(gamePathEntries, selectedEntry);
    }

    public void Dispose()
    {
        gameFileSystem?.Dispose();
    }
}