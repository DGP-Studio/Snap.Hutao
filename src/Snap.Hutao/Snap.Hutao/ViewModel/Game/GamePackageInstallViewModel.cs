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

    public Version? RemoteVersion { get; set => SetProperty(ref field, value, nameof(RemoteVersionText)); }

    public string RemoteVersionText { get => SH.FormatViewModelGamePackageRemoteVersion(RemoteVersion); }

    protected override async ValueTask<bool> LoadOverrideAsync()
    {
        LaunchScheme launchScheme = KnownLaunchSchemes.Get().First(scheme => scheme.IsNotCompatOnly);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HoyoPlayClient hoyoPlayClient = scope.ServiceProvider.GetRequiredService<HoyoPlayClient>();

            Response<GameBranchesWrapper> branchResp = await hoyoPlayClient.GetBranchesAsync(launchScheme).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(branchResp, serviceProvider, out GameBranchesWrapper? branchesWrapper))
            {
                return false;
            }

            if (branchesWrapper.GameBranches.FirstOrDefault(b => b.Game.Id == launchScheme.GameId) is { } branch)
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

        GameBranchesWrapper? branchesWrapper;
        GameChannelSDKsWrapper? channelSDKsWrapper;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HoyoPlayClient hoyoPlayClient = scope.ServiceProvider.GetRequiredService<HoyoPlayClient>();

            Response<GameBranchesWrapper> branchResp = await hoyoPlayClient.GetBranchesAsync(launchScheme).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(branchResp, serviceProvider, out branchesWrapper))
            {
                return;
            }

            Response<GameChannelSDKsWrapper> sdkResp = await hoyoPlayClient.GetChannelSDKAsync(launchScheme).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(sdkResp, serviceProvider, out channelSDKsWrapper))
            {
                return;
            }
        }

        GameBranch? branch = branchesWrapper.GameBranches.FirstOrDefault(b => b.Game.Id == launchScheme.GameId);
        GameChannelSDK? gameChannelSDK = channelSDKsWrapper.GameChannelSDKs.FirstOrDefault(sdk => sdk.Game.Id == launchScheme.GameId);

        ArgumentNullException.ThrowIfNull(branch);

        GamePackageOperationContext context = new(
            serviceProvider,
            GamePackageOperationKind.Install,
            gameFileSystem,
            default!,
            branch.Main,
            gameChannelSDK,
            default);

        gameFileSystem.GenerateConfigurationFile(branch.Main.Tag, launchScheme);

        if (!await gamePackageService.StartOperationAsync(context).ConfigureAwait(false))
        {
            // Operation canceled
        }
    }
}
