// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Package;
using Snap.Hutao.Service.Game.Package.Advanced;
using Snap.Hutao.Service.Game.Package.Advanced.Model;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using System.IO;

namespace Snap.Hutao.ViewModel.Game;

[GeneratedConstructor]
[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Singleton)]
internal sealed partial class GamePackageViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IGamePackageService gamePackageService;
    private readonly JsonSerializerOptions jsonOptions;
    private readonly LaunchGameShared launchGameShared;
    private readonly IServiceProvider serviceProvider;
    private readonly IHoyoPlayService hoyoPlayService;
    private readonly LaunchOptions launchOptions;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LocalVersionText), nameof(IsUpdateAvailable))]
    public partial Version? LocalVersion { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RemoteVersionText), nameof(IsUpdateAvailable))]
    public partial Version? RemoteVersion { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PreVersionText), nameof(IsPredownloadButtonEnabled))]
    public partial Version? PreVersion { get; set; }

    public string LocalVersionText { get => LocalVersion is null ? "Unknown" : SH.FormatViewModelGamePackageLocalVersion(LocalVersion); }

    public string RemoteVersionText { get => SH.FormatViewModelGamePackageRemoteVersion(RemoteVersion); }

    public string PreVersionText { get => SH.FormatViewModelGamePackagePreVersion(PreVersion); }

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

            const string LockTrace = $"{nameof(GamePackageViewModel)}.{nameof(IsPredownloadButtonEnabled)}";
            if (launchOptions.TryGetGameFileSystem(LockTrace, out IGameFileSystem? gameFileSystem) is not GameFileSystemErrorKind.None)
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
            const string LockTrace = $"{nameof(GamePackageViewModel)}.{nameof(IsPredownloadFinished)}";
            if (launchOptions.TryGetGameFileSystem(LockTrace, out IGameFileSystem? gameFileSystem) is not GameFileSystemErrorKind.None)
            {
                return false;
            }

            ArgumentNullException.ThrowIfNull(gameFileSystem);
            using (gameFileSystem)
            {
                if (!File.Exists(gameFileSystem.GetPredownloadStatusFilePath()))
                {
                    return false;
                }

                if (JsonSerializer.Deserialize<PredownloadStatus>(File.ReadAllText(gameFileSystem.GetPredownloadStatusFilePath()), jsonOptions) is { } predownloadStatus)
                {
                    int fileCount = Directory.GetFiles(gameFileSystem.GetChunksDirectory()).Length - 1;
                    return predownloadStatus.Finished && fileCount == predownloadStatus.TotalBlocks;
                }
            }

            return false;
        }
    }

    public async ValueTask ReloadAsync()
    {
        bool result = await LoadOverrideAsync(CancellationToken).ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();
        IsInitialized = result;
    }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (launchGameShared.GetCurrentLaunchSchemeFromConfigurationFile() is not { } launchScheme)
        {
            return false;
        }

        if (await GetCurrentGameBranchAsync(launchScheme).ConfigureAwait(false) is not { } branch)
        {
            return false;
        }

        await taskContext.SwitchToMainThreadAsync();

        (BranchWrapper remote, BranchWrapper? pre) = LocalSetting.Get(SettingKeys.TreatPredownloadAsMain, false)
            ? (branch.PreDownload ?? branch.Main, default)
            : (branch.Main, branch.PreDownload);

        (RemoteVersion, PreVersion) = (new(remote.Tag), pre is { Tag: { } preTag } ? new(preTag) : default);

        const string LockTrace = $"{nameof(GamePackageViewModel)}.{nameof(LoadOverrideAsync)}";
        if (launchOptions.TryGetGameFileSystem(LockTrace, out IGameFileSystem? gameFileSystem) is not GameFileSystemErrorKind.None)
        {
            return false;
        }

        ArgumentNullException.ThrowIfNull(gameFileSystem);
        using (gameFileSystem)
        {
            if (gameFileSystem.TryGetGameVersion(out string? localVersion))
            {
                _ = Version.TryParse(localVersion, out Version? version);
                LocalVersion = version;
            }

            if (!IsUpdateAvailable && PreVersion is null && File.Exists(gameFileSystem.GetPredownloadStatusFilePath()))
            {
                File.Delete(gameFileSystem.GetPredownloadStatusFilePath());
            }
        }

        return true;
    }

    [Command("StartCommand")]
    private async Task StartAsync(string? operation)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Start operation", "GamePackageViewModel.Command", [("operation", operation ?? "<null>")]));

        if (!IsInitialized)
        {
            return;
        }

        if (!Enum.TryParse(operation, out GamePackageOperationKind operationKind))
        {
            return;
        }

        if (launchGameShared.GetCurrentLaunchSchemeFromConfigurationFile() is not { } currentScheme)
        {
            return;
        }

        const string LockTrace = $"{nameof(GamePackageViewModel)}.{nameof(StartAsync)}";
        if (launchOptions.TryGetGameFileSystem(LockTrace, out IGameFileSystem? gameFileSystem) is not GameFileSystemErrorKind.None)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(gameFileSystem);
        using (gameFileSystem)
        {
            if (await GetCurrentGameBranchAsync(currentScheme).ConfigureAwait(false) is not { } branch)
            {
                return;
            }

            SophonDecodedBuilds? builds = await GetSophonDecodedBuildsAsync(operationKind, branch, gameFileSystem).ConfigureAwait(false);
            GameChannelSDK? sdk = await GetCurrentGameChannelSDKAsync(currentScheme).ConfigureAwait(false);
            GamePackageOperationContext context = new(serviceProvider, operationKind, gameFileSystem)
            {
                LocalBuild = builds?.LocalBuild,
                RemoteBuild = builds?.RemoteBuild,
                PatchBuild = builds?.PatchBuild,
                GameChannelSDK = sdk,
            };

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
                break;
            case GamePackageOperationKind.Predownload:
                OnPropertyChanged(nameof(IsPredownloadButtonEnabled));
                OnPropertyChanged(nameof(IsPredownloadFinished));
                break;
        }
    }

    private async ValueTask<SophonDecodedBuilds?> GetSophonDecodedBuildsAsync(GamePackageOperationKind operationKind, GameBranch branch, IGameFileSystem gameFileSystem)
    {
        ArgumentNullException.ThrowIfNull(LocalVersion);

        SophonDecodedBuild? localBuild;
        SophonDecodedBuild? remoteBuild;
        SophonDecodedPatchBuild? patchBuild;

        ContentDialog fetchManifestDialog = await contentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.UIXamlViewSpecializedSophonProgressDefault)
            .ConfigureAwait(false);
        using (await contentDialogFactory.BlockAsync(fetchManifestDialog).ConfigureAwait(false))
        {
            try
            {
                BranchWrapper localBranch = operationKind is GamePackageOperationKind.Verify && LocalSetting.Get(SettingKeys.TreatPredownloadAsMain, false)
                    ? branch.PreDownload?.GetTaggedCopy(LocalVersion.ToString()) ?? branch.Main.GetTaggedCopy(LocalVersion.ToString())
                    : branch.Main.GetTaggedCopy(LocalVersion.ToString());
                localBuild = await gamePackageService.DecodeManifestsAsync(gameFileSystem, localBranch).ConfigureAwait(false);

                BranchWrapper? remoteBranch = operationKind is GamePackageOperationKind.Update && LocalSetting.Get(SettingKeys.TreatPredownloadAsMain, false)
                    ? branch.PreDownload ?? branch.Main
                    : operationKind is GamePackageOperationKind.Predownload ? branch.PreDownload : branch.Main;
                remoteBuild = await gamePackageService.DecodeManifestsAsync(gameFileSystem, remoteBranch).ConfigureAwait(false);

                patchBuild = await gamePackageService.DecodeDiffManifestsAsync(gameFileSystem, remoteBranch).ConfigureAwait(false);

                ArgumentNullException.ThrowIfNull(localBuild);
                ArgumentNullException.ThrowIfNull(remoteBuild);
            }
            catch (Exception ex)
            {
                messenger.Send(InfoBarMessage.Error(ex));
                return default;
            }
        }

        return new()
        {
            LocalBuild = localBuild,
            RemoteBuild = remoteBuild,
            PatchBuild = patchBuild,
        };
    }

    private async ValueTask<GameBranch?> GetCurrentGameBranchAsync(LaunchScheme launchScheme)
    {
        if (await hoyoPlayService.TryGetBranchesAsync(launchScheme).ConfigureAwait(false) is not (true, { } branchesWrapper))
        {
            return default;
        }

        if (branchesWrapper.GameBranches.FirstOrDefault(b => b.Game.Id == launchScheme.GameId) is not { } branch)
        {
            messenger.Send(InfoBarMessage.Error(SH.ViewModelGamePackageGetGameBranchFailed, SH.FormatViewModelGamePackageLocalLaunchScheme(launchScheme.DisplayName)));
            return default;
        }

        return branch;
    }

    private async ValueTask<GameChannelSDK?> GetCurrentGameChannelSDKAsync(LaunchScheme launchScheme)
    {
        if (await hoyoPlayService.TryGetChannelSDKsAsync(launchScheme).ConfigureAwait(false) is not (true, { } channelSDKsWrapper))
        {
            return default;
        }

        // Channel sdk can be null
        return channelSDKsWrapper.GameChannelSDKs.FirstOrDefault(sdk => sdk.Game.Id == launchScheme.GameId);
    }

    private sealed class SophonDecodedBuilds
    {
        public required SophonDecodedBuild? LocalBuild { get; init; }

        public required SophonDecodedBuild? RemoteBuild { get; init; }

        public required SophonDecodedPatchBuild? PatchBuild { get; init; }
    }
}