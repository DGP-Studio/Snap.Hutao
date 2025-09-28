// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Property;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.ViewModel.Game;

[BindableCustomPropertyProvider]
internal sealed partial class LaunchSchemeFilteredGameAccountsView : ObservableObject
{
    private readonly AsyncLock syncRoot = new();

    private readonly IGameService gameService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    private LaunchScheme? scheme;

    public LaunchSchemeFilteredGameAccountsView(IProperty<bool> isViewUnloaded, IGameService gameService, ITaskContext taskContext, IMessenger messenger)
    {
        IsViewUnloaded = isViewUnloaded;
        this.gameService = gameService;
        this.taskContext = taskContext;
        this.messenger = messenger;
    }

    public LaunchScheme? Scheme
    {
        get => scheme;
        set => SetAsync(value, false).SafeForget();
    }

    [ObservableProperty]
    public partial IAdvancedCollectionView<GameAccount>? View { get; private set; }

    private IProperty<bool> IsViewUnloaded { get; }

    public async ValueTask SetAsync(LaunchScheme? value, bool external = true)
    {
        using (await syncRoot.LockAsync().ConfigureAwait(false))
        {
            await taskContext.SwitchToMainThreadAsync();
            if (!SetProperty(ref scheme, value, nameof(Scheme)))
            {
                return;
            }

            if (View is null)
            {
                IAdvancedCollectionView<GameAccount> accountsView = await gameService.GetGameAccountCollectionAsync().ConfigureAwait(true);
                await taskContext.SwitchToMainThreadAsync();
                View = accountsView;
            }
            else
            {
                // Clear the selected game account to prevent setting
                // incorrect CN/OS registry account when scheme not match
                await taskContext.SwitchToMainThreadAsync();
                View.MoveCurrentTo(default);
            }

            // Update GameAccountsView
            await taskContext.SwitchToMainThreadAsync();
            View.Filter = GameAccountFilter.Create(Scheme?.GetSchemeType());

            // Try set to the current registry account.
            if (Scheme is null)
            {
                if (external)
                {
                    messenger.Send(InfoBarMessage.Warning(SH.ViewModelLaunchGameSchemeNotSelected));
                }

                return;
            }

            if (View is null)
            {
                return;
            }

            // The GameAccount is guaranteed to be in the view, because the scheme is synced
            // Except when scheme is bilibili, which is not supported
            if (View.CurrentItem is null)
            {
                View.MoveCurrentTo(gameService.DetectCurrentGameAccountNoThrow(Scheme));
            }
        }
    }
}