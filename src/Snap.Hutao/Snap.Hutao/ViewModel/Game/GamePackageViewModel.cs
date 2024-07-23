// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Package;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Response;
using System.IO;

namespace Snap.Hutao.ViewModel.Game;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class GamePackageViewModel : Abstraction.ViewModel
{
    private readonly LaunchGameShared launchGameShared;
    private readonly LaunchOptions launchOptions;
    private readonly ITaskContext taskContext;
    private readonly IGamePackageService gamePackageService;
    private readonly HoyoPlayClient hoyoPlayClient;

    private GameBranch? gameBranch;
    private Version? localVersion;
    private Version? remoteVersion;
    private Version? preVersion;

    public GameBranch? GameBranch
    {
        get => gameBranch;
        set => SetProperty(ref gameBranch, value);
    }

    public Version? LocalVersion
    {
        get => localVersion;
        set => SetProperty(ref localVersion, value, nameof(LocalVersionText));
    }

    public Version? RemoteVersion
    {
        get => remoteVersion;
        set => SetProperty(ref remoteVersion, value, nameof(RemoteVersionText));
    }

    public Version? PreVersion
    {
        get => preVersion;
        set => SetProperty(ref preVersion, value, nameof(PreDownloadTitle));
    }

    public string LocalVersionText
    {
        get => LocalVersion is null ? "游戏未安装" : $"本地版本: {LocalVersion}";
    }

    public string RemoteVersionText
    {
        get => $"最新版本: {RemoteVersion}";
    }

    public string PreDownloadTitle
    {
        get => $"{PreVersion} 版本预下载已开启";
    }

    public bool IsUpdateAvailable
    {
        get => LocalVersion != RemoteVersion;
    }

    public bool IsPredownloadButtonEnabled
    {
        get => PreVersion is not null && !launchOptions.IsPredownloadFinished;
    }

    protected override async ValueTask<bool> InitializeOverrideAsync()
    {
        LaunchScheme? launchScheme = launchGameShared.GetCurrentLaunchSchemeFromConfigFile();
        if (launchScheme is null)
        {
            return false;
        }

        Response<GameBranchesWrapper> branchResp = await hoyoPlayClient.GetBranchesAsync(launchScheme).ConfigureAwait(false);
        if (!branchResp.IsOk())
        {
            return false;
        }

        if (branchResp.Data.GameBranches.FirstOrDefault(b => b.Game.Id == launchScheme.GameId) is { } branch)
        {
            await taskContext.SwitchToMainThreadAsync();
            GameBranch = branch;

            LocalVersion = launchOptions.TryGetGameFileSystem(out GameFileSystem? gameFileSystem)
                ? new(await File.ReadAllTextAsync(gameFileSystem.ScriptVersionFilePath).ConfigureAwait(true))
                : default;
            RemoteVersion = new(branch.Main.Tag);
            PreVersion = branch.PreDownload is { Tag: { } tag } ? new(tag) : default;

            return true;
        }

        return false;
    }

    [Command("StartCommand")]
    private async Task StartAsync(string state)
    {
        if (!IsInitialized)
        {
            return;
        }

        GamePackageOperationState targetState = Enum.Parse<GamePackageOperationState>(state);

        if (!launchOptions.TryGetGameFileSystem(out GameFileSystem? gameFileSystem))
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(GameBranch);
        ArgumentNullException.ThrowIfNull(LocalVersion);

        BranchWrapper remote = targetState switch
        {
            GamePackageOperationState.Predownload => GameBranch.PreDownload,
            _ => GameBranch.Main,
        };

        GamePackageOperationContext context = new(
            targetState,
            LaunchScheme.ExecutableIsOversea(gameFileSystem.GameFileName),
            gameFileSystem.GameDirectory,
            gameFileSystem.GameAudioSystem,
            GameBranch.Main.CloneWithTag(LocalVersion.ToString()),
            targetState is GamePackageOperationState.Predownload ? GameBranch.PreDownload : GameBranch.Main);
        await gamePackageService.StartOperationAsync(context).ConfigureAwait(false);
    }
}