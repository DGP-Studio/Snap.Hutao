// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Package.Advanced;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.ViewModel.Game;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class GamePackageInstallViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IGamePackageService gamePackageService;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    private Version? remoteVersion;

    public Version? RemoteVersion { get => remoteVersion; set => SetProperty(ref remoteVersion, value, nameof(RemoteVersionText)); }

    public string RemoteVersionText { get => SH.FormatViewModelGamePackageRemoteVersion(RemoteVersion); }

    protected override async ValueTask<bool> InitializeOverrideAsync()
    {
        LaunchScheme launchScheme = KnownLaunchSchemes.Get().First(scheme => scheme.IsNotCompatOnly);

        Response<GameBranchesWrapper> branchResp;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HoyoPlayClient hoyoPlayClient = scope.ServiceProvider.GetRequiredService<HoyoPlayClient>();

            branchResp = await hoyoPlayClient.GetBranchesAsync(launchScheme).ConfigureAwait(false);
            if (!branchResp.IsOk())
            {
                return false;
            }
        }

        if (branchResp.Data.GameBranches.FirstOrDefault(b => b.Game.Id == launchScheme.GameId) is { } branch)
        {
            await taskContext.SwitchToMainThreadAsync();
            RemoteVersion = new(branch.Main.Tag);
            return true;
        }

        return false;
    }

    [Command("StartCommand")]
    private async Task StartAsync()
    {
        if (!IsInitialized)
        {
            return;
        }

        LaunchGameInstallGameDialog dialog = await contentDialogFactory.CreateInstanceAsync<LaunchGameInstallGameDialog>().ConfigureAwait(false);
        dialog.KnownSchemes = KnownLaunchSchemes.Get();
        dialog.SelectedScheme = dialog.KnownSchemes.First(scheme => scheme.IsNotCompatOnly);
        (bool isOk, GameInstallOptions gameInstallOptions) = await dialog.GetGameFileSystemAsync().ConfigureAwait(false);

        if (!isOk)
        {
            return;
        }

        (GameFileSystem gameFileSystem, LaunchScheme launchScheme) = gameInstallOptions;

        Response<GameBranchesWrapper> branchResp;
        Response<GameChannelSDKsWrapper> sdkResp;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HoyoPlayClient hoyoPlayClient = scope.ServiceProvider.GetRequiredService<HoyoPlayClient>();

            branchResp = await hoyoPlayClient.GetBranchesAsync(launchScheme).ConfigureAwait(false);
            if (!branchResp.IsOk())
            {
                return;
            }

            sdkResp = await hoyoPlayClient.GetChannelSDKAsync(launchScheme).ConfigureAwait(false);
            if (!sdkResp.IsOk())
            {
                return;
            }
        }

        GameBranch? branch = branchResp.Data.GameBranches.FirstOrDefault(b => b.Game.Id == launchScheme.GameId);
        GameChannelSDK? gameChannelSDK = sdkResp.Data.GameChannelSDKs.FirstOrDefault(sdk => sdk.Game.Id == launchScheme.GameId);

        ArgumentNullException.ThrowIfNull(branch);

        GamePackageOperationContext context = new(
            serviceProvider,
            GamePackageOperationKind.Install,
            gameFileSystem,
            default!,
            branch.Main,
            gameChannelSDK);

        await gameFileSystem.ExtractConfigurationFileAsync(branch.Main.Tag, launchScheme).ConfigureAwait(false);

        if (!await gamePackageService.StartOperationAsync(context).ConfigureAwait(false))
        {
            // Operation canceled
            return;
        }
    }
}
