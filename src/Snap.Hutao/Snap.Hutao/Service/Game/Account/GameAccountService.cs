// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Dialog;

namespace Snap.Hutao.Service.Game.Account;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameAccountService))]
internal sealed partial class GameAccountService : IGameAccountService
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IInfoBarService infoBarService;
    private readonly IGameRepository gameRepository;
    private readonly ITaskContext taskContext;

    private readonly AsyncLock gameAccountLock = new();
    private IAdvancedCollectionView<GameAccount>? gameAccounts;

    public async ValueTask<IAdvancedCollectionView<GameAccount>> GetGameAccountCollectionAsync()
    {
        using (await gameAccountLock.LockAsync().ConfigureAwait(false))
        {
            if (gameAccounts is null)
            {
                await taskContext.SwitchToBackgroundAsync();
                gameAccounts = gameRepository.GetGameAccountCollection().AsAdvancedCollectionView();
            }
        }

        return gameAccounts;
    }

    public async ValueTask<GameAccount?> DetectCurrentGameAccountAsync(SchemeType schemeType)
    {
        ArgumentNullException.ThrowIfNull(gameAccounts);

        if (schemeType is SchemeType.ChineseBilibili)
        {
            throw HutaoException.NotSupported(SH.ServiceGameAccountBilibiliNotSupported);
        }

        string? registrySdk = RegistryInterop.Get(schemeType);
        if (string.IsNullOrEmpty(registrySdk))
        {
            return default;
        }

        if (SingleGameAccountOrDefault(gameAccounts.Source.AsReadOnly(), registrySdk) is { } account)
        {
            return account;
        }

        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            LaunchGameAccountNameDialog dialog = await contentDialogFactory.CreateInstanceAsync<LaunchGameAccountNameDialog>(scope.ServiceProvider).ConfigureAwait(false);
            if (await dialog.GetInputNameAsync().ConfigureAwait(false) is not (true, { } name))
            {
                // User cancelled the dialog
                return default;
            }

            if (gameAccounts.Source.Any(a => a.Name == name))
            {
                infoBarService.Warning(SH.FormatServiceGameAccountDetectInputNameAlreadyExists(name));
                return default;
            }

            account = GameAccount.Create(name, registrySdk, schemeType);
        }

        await taskContext.SwitchToBackgroundAsync();
        gameRepository.AddGameAccount(account);

        await taskContext.SwitchToMainThreadAsync();
        gameAccounts.Add(account);

        return account;
    }

    public GameAccount? DetectCurrentGameAccount(SchemeType schemeType)
    {
        ArgumentNullException.ThrowIfNull(gameAccounts);

        if (schemeType is SchemeType.ChineseBilibili)
        {
            throw HutaoException.Throw(SH.ServiceGameAccountBilibiliNotSupported);
        }

        string? registrySdk = RegistryInterop.Get(schemeType);

        return string.IsNullOrEmpty(registrySdk) ? default : SingleGameAccountOrDefault(gameAccounts.Source.AsReadOnly(), registrySdk);
    }

    public bool SetGameAccount(GameAccount account)
    {
        return RegistryInterop.Set(account);
    }

    public async ValueTask ModifyGameAccountAsync(GameAccount gameAccount)
    {
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            LaunchGameAccountNameDialog dialog = await contentDialogFactory.CreateInstanceAsync<LaunchGameAccountNameDialog>(scope.ServiceProvider).ConfigureAwait(false);

            if (await dialog.GetInputNameAsync().ConfigureAwait(false) is (true, { } name))
            {
                await taskContext.SwitchToMainThreadAsync();
                gameAccount.UpdateName(name);

                await taskContext.SwitchToBackgroundAsync();
                gameRepository.UpdateGameAccount(gameAccount);
            }
        }
    }

    public async ValueTask RemoveGameAccountAsync(GameAccount gameAccount)
    {
        ArgumentNullException.ThrowIfNull(gameAccounts);

        await taskContext.SwitchToMainThreadAsync();
        gameAccounts.Remove(gameAccount);

        await taskContext.SwitchToBackgroundAsync();
        gameRepository.RemoveGameAccountById(gameAccount.InnerId);
    }

    private static GameAccount? SingleGameAccountOrDefault(IReadOnlyCollection<GameAccount> gameAccounts, string registrySdk)
    {
        try
        {
            return gameAccounts.SingleOrDefault(a => a.MihoyoSDK == registrySdk);
        }
        catch (InvalidOperationException ex)
        {
            throw HutaoException.Throw(SH.ServiceGameDetectGameAccountMultiMatched, ex);
        }
    }
}