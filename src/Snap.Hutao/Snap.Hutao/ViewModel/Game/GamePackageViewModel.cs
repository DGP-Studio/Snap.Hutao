// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Package.Advanced;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
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
    private readonly IServiceProvider serviceProvider;
    private readonly LaunchOptions launchOptions;
    private readonly ITaskContext taskContext;

    public Version? LocalVersion
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(LocalVersionText));
                OnPropertyChanged(nameof(IsUpdateAvailable));
            }
        }
    }

    public Version? RemoteVersion
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(RemoteVersionText));
                OnPropertyChanged(nameof(IsUpdateAvailable));
            }
        }
    }

    public Version? PreVersion
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(PreDownloadTitle));
                OnPropertyChanged(nameof(IsPredownloadButtonEnabled));
            }
        }
    }

    public string LocalVersionText { get => LocalVersion is null ? "Unknown" : SH.FormatViewModelGamePackageLocalVersion(LocalVersion); }

    public string RemoteVersionText { get => SH.FormatViewModelGamePackageRemoteVersion(RemoteVersion); }

    public string PreDownloadTitle { get => SH.FormatViewModelGamePackagePreVersion(PreVersion); }

    public bool IsUpdateAvailable { get => LocalVersion < RemoteVersion; }

    public bool IsPredownloadButtonEnabled
    {
        get
        {
            if (PreVersion is null)
            {
                return false;
            }

            if (LocalVersion >= PreVersion)
            {
                return false;
            }

            if (!launchOptions.TryGetGameFileSystem(out IGameFileSystem? gameFileSystem))
            {
                return false;
            }

            using (gameFileSystem)
            {
                // IsPredownloadFinished also TryGetGameFileSystem
                return !IsPredownloadFinished;
            }
        }
    }

    public bool IsPredownloadFinished
    {
        get
        {
            if (!launchOptions.TryGetGameFileSystem(out IGameFileSystem? gameFileSystem))
            {
                return false;
            }

            using (gameFileSystem)
            {
                if (!File.Exists(gameFileSystem.GetPredownloadStatusPath()))
                {
                    return false;
                }

                if (JsonSerializer.Deserialize<PredownloadStatus>(File.ReadAllText(gameFileSystem.GetPredownloadStatusPath())) is { } predownloadStatus)
                {
                    int fileCount = Directory.GetFiles(gameFileSystem.GetChunksDirectory()).Length - 1;
                    return predownloadStatus.Finished && fileCount == predownloadStatus.TotalBlocks;
                }
            }

            return false;
        }
    }

    public async ValueTask ForceLoadAsync()
    {
        bool result = await LoadOverrideAsync(CancellationToken).ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();
        IsInitialized = result;
    }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (launchGameShared.GetCurrentLaunchSchemeFromConfigFile() is not { } launchScheme)
        {
            return false;
        }

        if (await GetCurrentGameBranchAsync(launchScheme).ConfigureAwait(false) is not { } branch)
        {
            return false;
        }

        await taskContext.SwitchToMainThreadAsync();

        if (LocalSetting.Get(SettingKeys.TreatPredownloadAsMain, false))
        {
            BranchWrapper remoteBranch = branch.PreDownload ?? branch.Main;
            RemoteVersion = new(remoteBranch.Tag);
            PreVersion = default;
        }
        else
        {
            RemoteVersion = new(branch.Main.Tag);
            PreVersion = branch.PreDownload is { Tag: { } tag } ? new(tag) : default;
        }

        if (!launchOptions.TryGetGameFileSystem(out IGameFileSystem? gameFileSystem))
        {
            return false;
        }

        using (gameFileSystem)
        {
            if (gameFileSystem.TryGetGameVersion(out string? localVersion))
            {
                Version.TryParse(localVersion, out Version? version);
                LocalVersion = version;
            }

            if (!IsUpdateAvailable && PreVersion is null && File.Exists(gameFileSystem.GetPredownloadStatusPath()))
            {
                File.Delete(gameFileSystem.GetPredownloadStatusPath());
            }
        }

        return true;
    }

    [Command("StartCommand")]
    private async Task StartAsync(string operation)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Start operation", "GamePackageViewModel.Command", [("operation", operation)]));

        if (!IsInitialized)
        {
            return;
        }

        if (launchGameShared.GetCurrentLaunchSchemeFromConfigFile() is not { } launchScheme)
        {
            return;
        }

        if (!launchOptions.TryGetGameFileSystem(out IGameFileSystem? gameFileSystem))
        {
            return;
        }

        GamePackageOperationKind operationKind = Enum.Parse<GamePackageOperationKind>(operation);

        using (gameFileSystem)
        {
            ArgumentNullException.ThrowIfNull(LocalVersion);

            GameBranch? branch = await GetCurrentGameBranchAsync(launchScheme).ConfigureAwait(false);
            if (branch is null)
            {
                return;
            }

            SophonDecodedBuild? localBuild;
            SophonDecodedBuild? remoteBuild;

            ContentDialog fetchManifestDialog = await contentDialogFactory
                .CreateForIndeterminateProgressAsync(SH.UIXamlViewSpecializedSophonProgressDefault)
                .ConfigureAwait(false);
            using (await contentDialogFactory.BlockAsync(fetchManifestDialog).ConfigureAwait(false))
            {
                try
                {
                    BranchWrapper localBranch = branch.Main.GetTaggedCopy(LocalVersion.ToString());
                    localBuild = await gamePackageService.DecodeManifestsAsync(gameFileSystem, localBranch).ConfigureAwait(false);

                    BranchWrapper? remoteBranch = operationKind is GamePackageOperationKind.Update && LocalSetting.Get(SettingKeys.TreatPredownloadAsMain, false)
                        ? branch.PreDownload ?? branch.Main
                        : operationKind is GamePackageOperationKind.Predownload ? branch.PreDownload : branch.Main;
                    remoteBuild = await gamePackageService.DecodeManifestsAsync(gameFileSystem, remoteBranch).ConfigureAwait(false);

                    ArgumentNullException.ThrowIfNull(localBuild);
                    ArgumentNullException.ThrowIfNull(remoteBuild);
                }
                catch (Exception ex)
                {
                    serviceProvider.GetRequiredService<IInfoBarService>().Error(ex);
                    return;
                }
            }

            GamePackageOperationContext context = new(
                serviceProvider,
                operationKind,
                gameFileSystem,
                localBuild,
                remoteBuild,
                await GetCurrentGameChannelSDKAsync(launchScheme).ConfigureAwait(false),
                default);

            if (!await gamePackageService.ExecuteOperationAsync(context).ConfigureAwait(false))
            {
                // Operation canceled
                return;
            }
        }

        await taskContext.SwitchToMainThreadAsync();

        switch (operationKind)
        {
            case GamePackageOperationKind.Verify:
                break;
            case GamePackageOperationKind.Update:
                LocalVersion = RemoteVersion;
                OnPropertyChanged(nameof(IsUpdateAvailable));
                break;
            case GamePackageOperationKind.Predownload:
                OnPropertyChanged(nameof(IsPredownloadButtonEnabled));
                OnPropertyChanged(nameof(IsPredownloadFinished));
                break;
        }
    }

    private async ValueTask<GameBranch?> GetCurrentGameBranchAsync(LaunchScheme launchScheme)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HoyoPlayClient hoyoPlayClient = scope.ServiceProvider.GetRequiredService<HoyoPlayClient>();
            Response<GameBranchesWrapper> branchResp = await hoyoPlayClient.GetBranchesAsync(launchScheme).ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(branchResp, scope.ServiceProvider, out GameBranchesWrapper? branchesWrapper))
            {
                return default;
            }

            if (branchesWrapper.GameBranches.FirstOrDefault(b => b.Game.Id == launchScheme.GameId) is not { } branch)
            {
                serviceProvider.GetRequiredService<IInfoBarService>().Error(SH.ViewModelGamePackageGetGameBranchFailed, SH.FormatViewModelGamePackageLocalLaunchScheme(launchScheme.DisplayName));
                return default;
            }

            return branch;
        }
    }

    private async ValueTask<GameChannelSDK?> GetCurrentGameChannelSDKAsync(LaunchScheme launchScheme)
    {
        GameChannelSDKsWrapper? channelSDKsWrapper;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HoyoPlayClient hoyoPlayClient = scope.ServiceProvider.GetRequiredService<HoyoPlayClient>();
            Response<GameChannelSDKsWrapper> sdkResp = await hoyoPlayClient.GetChannelSDKAsync(launchScheme).ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(sdkResp, scope.ServiceProvider, out channelSDKsWrapper))
            {
                return default;
            }
        }

        // Channel sdk can be null
        return channelSDKsWrapper.GameChannelSDKs.FirstOrDefault(sdk => sdk.Game.Id == launchScheme.GameId);
    }
}