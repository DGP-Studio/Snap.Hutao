// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Package;
using Snap.Hutao.Service.Game.Package.Advanced;
using Snap.Hutao.Service.Game.Package.Advanced.Model;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.Web.Hoyolab.Downloader;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.ViewModel.Game;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Singleton)]
internal sealed partial class GamePackageInstallViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IGamePackageService gamePackageService;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial GamePackageInstallViewModel(IServiceProvider serviceProvider);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RemoteVersionText))]
    public partial Version? RemoteVersion { get; set; }

    public string RemoteVersionText { get => SH.FormatViewModelGamePackageRemoteVersion(RemoteVersion); }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HoyoPlayClient hoyoPlayClient = scope.ServiceProvider.GetRequiredService<HoyoPlayClient>();

            if (await TrySetRemoteVersionAsync(scope.ServiceProvider, hoyoPlayClient, false, token).ConfigureAwait(false))
            {
                return true;
            }

            if (await TrySetRemoteVersionAsync(scope.ServiceProvider, hoyoPlayClient, true, token).ConfigureAwait(false))
            {
                return true;
            }
        }

        return false;
    }

    private async ValueTask<bool> TrySetRemoteVersionAsync(IServiceProvider serviceProvider, HoyoPlayClient hoyoPlayClient, bool isOversea, CancellationToken token)
    {
        LaunchScheme scheme = KnownLaunchSchemes.EnumerateNotCompatOnly(isOversea).First();
        Response<GameBranchesWrapper> response = await hoyoPlayClient.GetBranchesAsync(scheme, token).ConfigureAwait(false);
        if (ResponseValidator.TryValidate(response, serviceProvider, out GameBranchesWrapper? branchesWrapper))
        {
            if (branchesWrapper.GameBranches.FirstOrDefault(b => string.Equals(b.Game.Id, scheme.GameId, StringComparison.OrdinalIgnoreCase)) is { } branch)
            {
                await taskContext.SwitchToMainThreadAsync();
                RemoteVersion = new(branch.Main.Tag);
                return true;
            }
        }

        return false;
    }

    [Command("StartCommand")]
    private async Task StartAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Start install operation", "GamePackageInstallViewModel.Command"));

        if (!IsInitialized)
        {
            return;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            LaunchGameInstallGameDialog installOptionsDialog = await contentDialogFactory.CreateInstanceAsync<LaunchGameInstallGameDialog>(scope.ServiceProvider).ConfigureAwait(false);
            if (await installOptionsDialog.GetGameInstallOptionsAsync().ConfigureAwait(false) is not (true, { } gameInstallOptions))
            {
                return;
            }

            IGameFileSystem gameFileSystem;
            LaunchScheme launchScheme;
            SophonDecodedBuild? build;
            GameChannelSDK? gameChannelSDK = default;
            string installLockVersionTag;

            ContentDialog fetchManifestDialog = await contentDialogFactory
                .CreateForIndeterminateProgressAsync(SH.UIXamlViewSpecializedSophonProgressDefault)
                .ConfigureAwait(false);

            using (await contentDialogFactory.BlockAsync(fetchManifestDialog).ConfigureAwait(false))
            {
                if (gameInstallOptions.IsBeta)
                {
                    (gameFileSystem, launchScheme, SophonBuild betaBuild) = gameInstallOptions;

                    build = await gamePackageService.DecodeManifestsAsync(gameFileSystem, betaBuild).ConfigureAwait(false);
                    if (build is null)
                    {
                        messenger.Send(InfoBarMessage.Error(SH.ServiceGamePackageAdvancedDecodeManifestFailed));
                        return;
                    }

                    installLockVersionTag = betaBuild.Tag;
                }
                else
                {
                    (gameFileSystem, launchScheme) = gameInstallOptions;

                    IHoyoPlayService hoyoPlayService = scope.ServiceProvider.GetRequiredService<IHoyoPlayService>();

                    if (await hoyoPlayService.TryGetBranchesAsync(launchScheme).ConfigureAwait(false) is not (true, { } branchesWrapper))
                    {
                        messenger.Send(InfoBarMessage.Error(SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed("Target Branches")));
                        return;
                    }

                    GameBranch branch = branchesWrapper.GameBranches.First(b => b.Game.Id == launchScheme.GameId);

                    if (await hoyoPlayService.TryGetChannelSDKsAsync(launchScheme).ConfigureAwait(false) is not (true, { } channelSDKsWrapper))
                    {
                        messenger.Send(InfoBarMessage.Error(SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed("Target Channel SDKs")));
                        return;
                    }

                    gameChannelSDK = channelSDKsWrapper.GameChannelSDKs.FirstOrDefault(sdk => sdk.Game.Id == launchScheme.GameId);

                    build = await gamePackageService.DecodeManifestsAsync(gameFileSystem, branch.Main).ConfigureAwait(false);
                    if (build is null)
                    {
                        messenger.Send(InfoBarMessage.Error(SH.ServiceGamePackageAdvancedDecodeManifestFailed));
                        return;
                    }

                    installLockVersionTag = branch.Main.Tag;
                }
            }

            if (!GameInstallPersistence.TryAcquire(gameFileSystem, installLockVersionTag, launchScheme, out GameInstallPersistence? persistence))
            {
                messenger.Send(InfoBarMessage.Error(SH.ViewDialogLaunchGameInstallGameDirectoryExistsFileSystemEntry));
                return;
            }

            GamePackageOperationContext context = new(serviceProvider, GamePackageOperationKind.Install, gameFileSystem)
            {
                RemoteBuild = build,
                GameChannelSDK = gameChannelSDK,
            };

            if (!await gamePackageService.ExecuteOperationAsync(context).ConfigureAwait(false))
            {
                // Operation canceled or failed
                return;
            }

            persistence.Release();
        }
    }
}