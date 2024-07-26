// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Package;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Response;
using System.IO;

namespace Snap.Hutao.ViewModel.Game;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class GamePackageViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IGamePackageService gamePackageService;
    private readonly LaunchGameShared launchGameShared;
    private readonly HoyoPlayClient hoyoPlayClient;
    private readonly LaunchOptions launchOptions;
    private readonly ITaskContext taskContext;

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
        get
        {
            if (PreVersion is null)
            {
                return false;
            }

            if (!launchOptions.TryGetGameFileSystem(out GameFileSystem? gameFileSystem))
            {
                return false;
            }

            return !IsPredownloadFinished;
        }
    }

    public bool IsPredownloadFinished
    {
        get
        {
            if (!launchOptions.TryGetGameFileSystem(out GameFileSystem? gameFileSystem))
            {
                return false;
            }

            string predownloadStatusPath = Path.Combine(gameFileSystem.ChunksDirectory, "snap_hutao_predownload_status.json");

            if (!File.Exists(predownloadStatusPath))
            {
                return false;
            }

            PredownloadStatus? predownloadStatus = JsonSerializer.Deserialize<PredownloadStatus>(File.ReadAllText(predownloadStatusPath));
            if (predownloadStatus is { })
            {
                int fileCount = Directory.GetFiles(gameFileSystem.ChunksDirectory).Length - 1;
                return predownloadStatus.Finished && fileCount == predownloadStatus.TotalBlocks;
            }

            return false;
        }
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

            RemoteVersion = new(branch.Main.Tag);
            PreVersion = branch.PreDownload is { Tag: { } tag } ? new(tag) : default;

            if (!launchOptions.TryGetGameFileSystem(out GameFileSystem? gameFileSystem))
            {
                return true;
            }

            if (File.Exists(gameFileSystem.ScriptVersionFilePath))
            {
                LocalVersion = new(await File.ReadAllTextAsync(gameFileSystem.ScriptVersionFilePath).ConfigureAwait(true));
            }

            if (!IsUpdateAvailable && PreVersion is null && File.Exists(gameFileSystem.PredownloadStatusPath))
            {
                File.Delete(gameFileSystem.PredownloadStatusPath);
            }

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

        if (targetState is GamePackageOperationState.Install)
        {
            LaunchGameInstallGameDialog dialog = await contentDialogFactory.CreateInstanceAsync<LaunchGameInstallGameDialog>().ConfigureAwait(false);
            dialog.KnownSchemes = KnownLaunchSchemes.Get();
            dialog.SelectedScheme = dialog.KnownSchemes.First(scheme => scheme.IsNotCompatOnly);
            (bool isOk, gameFileSystem) = await dialog.GetGameFileSystemAsync().ConfigureAwait(false);

            if (!isOk)
            {
                return;
            }
        }

        GamePackageOperationContext context = new(
            targetState,
            gameFileSystem,
            GameBranch.Main.CloneWithTag(LocalVersion.ToString()),
            targetState is GamePackageOperationState.Predownload ? GameBranch.PreDownload : GameBranch.Main);

        bool success = await gamePackageService.StartOperationAsync(context).ConfigureAwait(false);

        await taskContext.SwitchToMainThreadAsync();
        if (success)
        {
            switch (targetState)
            {
                case GamePackageOperationState.Verify:
                    break;
                case GamePackageOperationState.Update:
                    LocalVersion = RemoteVersion;
                    OnPropertyChanged(nameof(IsUpdateAvailable));
                    break;
                case GamePackageOperationState.Predownload:
                    OnPropertyChanged(nameof(IsPredownloadButtonEnabled));
                    OnPropertyChanged(nameof(IsPredownloadFinished));
                    break;
            }
        }
    }
}