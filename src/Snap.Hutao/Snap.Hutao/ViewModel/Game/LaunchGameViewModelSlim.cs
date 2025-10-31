// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Property;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Package;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.UI.Xaml.View.Page;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using System.Diagnostics;

namespace Snap.Hutao.ViewModel.Game;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Transient)]
internal sealed partial class LaunchGameViewModelSlim : Abstraction.ViewModelSlim<LaunchGamePage>,
    IViewModelSupportLaunchExecution,
    IRecipient<UserAndUidChangedMessage>
{
    private readonly IServiceProvider serviceProvider;
    private readonly IGameService gameService;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial LaunchGameViewModelSlim(IServiceProvider serviceProvider);

    public partial LaunchGameShared Shared { get; }

    public partial LaunchStatusOptions LaunchStatusOptions { get; }

    public partial LaunchOptions LaunchOptions { get; }

    public LaunchSchemeFilteredGameAccountsView CurrentSchemeFilteredGameAccountsView { get => field ??= new(Property.Create(false), gameService, taskContext, messenger); }

    [ObservableProperty]
    public partial UserGameRole? CurrentUserGameRole { get; set; }

    LaunchScheme? IViewModelSupportLaunchExecution.TargetScheme { get => CurrentSchemeFilteredGameAccountsView.Scheme; }

    LaunchScheme? IViewModelSupportLaunchExecution.CurrentScheme { get => CurrentSchemeFilteredGameAccountsView.Scheme; }

    GameAccount? IViewModelSupportLaunchExecution.GameAccount { get => CurrentSchemeFilteredGameAccountsView.View?.CurrentItem; }

    ValueTask<BlockDeferral<PackageConvertStatus>> IViewModelSupportLaunchExecution.CreateConvertBlockDeferralAsync()
    {
        // Should never happen: slim does not support package conversion.
        Debugger.Break();
        return BlockDeferral<PackageConvertStatus>.CreateAsync<LaunchGamePackageConvertDialog>(serviceProvider, static (state, dialog) => dialog.State = state);
    }

    public void Receive(UserAndUidChangedMessage message)
    {
        // There is no way to change UsingHoyolabAccount when viewing cards.
        // So we don't need to update CurrentUserGameRole when UsingHoyolabAccount changed.
        if (!LaunchOptions.UsingHoyolabAccount.Value)
        {
            return;
        }

        taskContext.InvokeOnMainThread(() =>
        {
            // We will fetch the UserGameRole when loading.
            if (!IsInitialized)
            {
                return;
            }

            CurrentUserGameRole = message.User?.UserGameRoles.CurrentItem;
        });
    }

    protected override async Task LoadAsync()
    {
        if (Shared.GetCurrentLaunchSchemeFromConfigurationFile() is not { } scheme)
        {
            // The scheme is null, user must go to LaunchGamePage to select a proper game path.
            // And even try to fix the configuration file.
            return;
        }

        await CurrentSchemeFilteredGameAccountsView.SetAsync(scheme).ConfigureAwait(true);
        Shared.ResumeLaunchExecutionAsync(this).SafeForget();

        UserGameRole? userGameRole = LaunchOptions.UsingHoyolabAccount.Value
            ? await userService.GetCurrentUserGameRoleAsync().ConfigureAwait(false)
            : default;

        await taskContext.SwitchToMainThreadAsync();
        CurrentUserGameRole = userGameRole;
        IsInitialized = true;
    }

    [Command("LaunchCommand")]
    private async Task LaunchAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Launch game", "LaunchGameViewModelSlim.Command"));

        UserAndUid? userAndUid = await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false);
        await Shared.DefaultLaunchExecutionAsync(this, userAndUid).ConfigureAwait(false);
    }
}