﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.UI.Xaml.View.Dialog;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Game.Account;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameAccountService))]
internal sealed partial class GameAccountService : IGameAccountService
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IGameRepository gameRepository;
    private readonly ITaskContext taskContext;

    private ObservableReorderableDbCollection<GameAccount>? gameAccounts;

    public async ValueTask<ObservableReorderableDbCollection<GameAccount>> GetGameAccountCollectionAsync()
    {
        if (gameAccounts is null)
        {
            await taskContext.SwitchToBackgroundAsync();
            gameAccounts = gameRepository.GetGameAccountCollection();
        }

        return gameAccounts;
    }

    public async ValueTask<GameAccount?> DetectGameAccountAsync(SchemeType schemeType)
    {
        ArgumentNullException.ThrowIfNull(gameAccounts);

        if (schemeType is SchemeType.ChineseBilibili)
        {
            return default;
        }

        string? registrySdk = RegistryInterop.Get(schemeType);
        if (string.IsNullOrEmpty(registrySdk))
        {
            return default;
        }

        GameAccount? account = SingleGameAccountOrDefault(gameAccounts, registrySdk);
        if (account is null)
        {
            LaunchGameAccountNameDialog dialog = await contentDialogFactory.CreateInstanceAsync<LaunchGameAccountNameDialog>().ConfigureAwait(false);
            (bool isOk, string name) = await dialog.GetInputNameAsync().ConfigureAwait(false);

            if (isOk)
            {
                account = GameAccount.From(name, registrySdk, schemeType);

                // sync database
                await taskContext.SwitchToBackgroundAsync();
                gameRepository.AddGameAccount(account);

                // sync cache
                await taskContext.SwitchToMainThreadAsync();
                gameAccounts.Add(account);
            }
        }

        return account;
    }

    public GameAccount? DetectCurrentGameAccount(SchemeType schemeType)
    {
        ArgumentNullException.ThrowIfNull(gameAccounts);

        if (schemeType is SchemeType.ChineseBilibili)
        {
            return default;
        }

        string? registrySdk = RegistryInterop.Get(schemeType);

        return string.IsNullOrEmpty(registrySdk) ? default : SingleGameAccountOrDefault(gameAccounts, registrySdk);
    }

    public bool SetGameAccount(GameAccount account)
    {
        return RegistryInterop.Set(account);
    }

    public async ValueTask ModifyGameAccountAsync(GameAccount gameAccount)
    {
        LaunchGameAccountNameDialog dialog = await contentDialogFactory.CreateInstanceAsync<LaunchGameAccountNameDialog>().ConfigureAwait(false);
        (bool isOk, string name) = await dialog.GetInputNameAsync().ConfigureAwait(false);

        if (isOk)
        {
            await taskContext.SwitchToMainThreadAsync();
            gameAccount.UpdateName(name);

            // sync database
            await taskContext.SwitchToBackgroundAsync();
            gameRepository.UpdateGameAccount(gameAccount);
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

    private static GameAccount? SingleGameAccountOrDefault(ObservableCollection<GameAccount> gameAccounts, string registrySdk)
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