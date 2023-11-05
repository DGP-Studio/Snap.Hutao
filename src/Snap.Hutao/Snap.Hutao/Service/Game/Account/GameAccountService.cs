// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.View.Dialog;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Game.Account;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameAccountService))]
internal sealed partial class GameAccountService : IGameAccountService
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IGameDbService gameDbService;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;

    private ObservableCollection<GameAccount>? gameAccounts;

    public ObservableCollection<GameAccount> GameAccountCollection
    {
        get => gameAccounts ??= gameDbService.GetGameAccountCollection();
    }

    public async ValueTask<GameAccount?> DetectGameAccountAsync()
    {
        ArgumentNullException.ThrowIfNull(gameAccounts);

        string? registrySdk = RegistryInterop.Get();
        if (!string.IsNullOrEmpty(registrySdk))
        {
            GameAccount? account = null;
            try
            {
                account = gameAccounts.SingleOrDefault(a => a.MihoyoSDK == registrySdk);
            }
            catch (InvalidOperationException ex)
            {
                ThrowHelper.UserdataCorrupted(SH.ServiceGameDetectGameAccountMultiMatched, ex);
            }

            if (account is null)
            {
                LaunchGameAccountNameDialog dialog = await contentDialogFactory.CreateInstanceAsync<LaunchGameAccountNameDialog>().ConfigureAwait(false);
                (bool isOk, string name) = await dialog.GetInputNameAsync().ConfigureAwait(false);

                if (isOk)
                {
                    account = GameAccount.From(name, registrySdk);

                    // sync database
                    await taskContext.SwitchToBackgroundAsync();
                    await gameDbService.AddGameAccountAsync(account).ConfigureAwait(false);

                    // sync cache
                    await taskContext.SwitchToMainThreadAsync();
                    gameAccounts.Add(account);
                }
            }

            return account;
        }

        return default;
    }

    public GameAccount? DetectCurrentGameAccount()
    {
        ArgumentNullException.ThrowIfNull(gameAccounts);

        string? registrySdk = RegistryInterop.Get();

        if (!string.IsNullOrEmpty(registrySdk))
        {
            try
            {
                return gameAccounts.SingleOrDefault(a => a.MihoyoSDK == registrySdk);
            }
            catch (InvalidOperationException ex)
            {
                ThrowHelper.UserdataCorrupted(SH.ServiceGameDetectGameAccountMultiMatched, ex);
            }
        }

        return null;
    }

    public bool SetGameAccount(GameAccount account)
    {
        if (string.IsNullOrEmpty(appOptions.PowerShellPath))
        {
            ThrowHelper.RuntimeEnvironment(SH.ServiceGameRegisteryInteropPowershellNotFound, default!);
        }

        return RegistryInterop.Set(account, appOptions.PowerShellPath);
    }

    public void AttachGameAccountToUid(GameAccount gameAccount, string uid)
    {
        gameAccount.UpdateAttachUid(uid);
        gameDbService.UpdateGameAccount(gameAccount);
    }

    public async ValueTask ModifyGameAccountAsync(GameAccount gameAccount)
    {
        await taskContext.SwitchToMainThreadAsync();
        LaunchGameAccountNameDialog dialog = serviceProvider.CreateInstance<LaunchGameAccountNameDialog>();
        (bool isOk, string name) = await dialog.GetInputNameAsync().ConfigureAwait(true);

        if (isOk)
        {
            gameAccount.UpdateName(name);

            // sync database
            await taskContext.SwitchToBackgroundAsync();
            await gameDbService.UpdateGameAccountAsync(gameAccount).ConfigureAwait(false);
        }
    }

    public async ValueTask RemoveGameAccountAsync(GameAccount gameAccount)
    {
        await taskContext.SwitchToMainThreadAsync();
        ArgumentNullException.ThrowIfNull(gameAccounts);
        gameAccounts.Remove(gameAccount);

        await taskContext.SwitchToBackgroundAsync();
        await gameDbService.RemoveGameAccountByIdAsync(gameAccount.InnerId).ConfigureAwait(false);
    }
}