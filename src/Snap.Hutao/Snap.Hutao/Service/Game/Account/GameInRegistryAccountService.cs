// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.Service.Game.Account;

[ConstructorGenerated]
[Service(ServiceLifetime.Singleton, typeof(IGameInRegistryAccountService))]
internal sealed partial class GameInRegistryAccountService : IGameInRegistryAccountService
{
    private readonly IGameAccountRepository gameAccountRepository;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    private readonly AsyncLock gameAccountLock = new();
    private IAdvancedCollectionView<GameAccount>? gameAccounts;

    public async ValueTask<IAdvancedCollectionView<GameAccount>> GetGameAccountCollectionAsync()
    {
        using (await gameAccountLock.LockAsync().ConfigureAwait(false))
        {
            if (gameAccounts is null)
            {
                await taskContext.SwitchToBackgroundAsync();
                gameAccounts = gameAccountRepository.GetGameAccountCollection().AsAdvancedCollectionView();
            }
        }

        return gameAccounts;
    }

    public async ValueTask<GameAccount?> DetectCurrentGameAccountAsync(SchemeType schemeType, Func<Task<ValueResult<bool, string?>>> providerNameCallback)
    {
        ArgumentNullException.ThrowIfNull(gameAccounts);
        HutaoException.NotSupportedIf(schemeType is SchemeType.ChineseBilibili, SH.ServiceGameAccountBilibiliNotSupported);

        string? registrySdk = RegistryInterop.Get(schemeType);
        if (string.IsNullOrEmpty(registrySdk))
        {
            return default;
        }

        if (SingleGameAccountOrDefault(gameAccounts.Source.AsReadOnly(), registrySdk) is { } account)
        {
            return account;
        }

        if (await providerNameCallback().ConfigureAwait(false) is not (true, { } name))
        {
            return default;
        }

        if (gameAccounts.Source.Any(a => a.Name == name))
        {
            messenger.Send(InfoBarMessage.Warning(SH.FormatServiceGameAccountDetectInputNameAlreadyExists(name)));
            return default;
        }

        account = GameAccount.Create(name, registrySdk, schemeType);

        await taskContext.SwitchToBackgroundAsync();
        gameAccountRepository.AddGameAccount(account);

        await taskContext.SwitchToMainThreadAsync();
        gameAccounts.Add(account);

        return account;
    }

    public GameAccount? DetectCurrentGameAccount(SchemeType schemeType)
    {
        ArgumentNullException.ThrowIfNull(gameAccounts);
        HutaoException.NotSupportedIf(schemeType is SchemeType.ChineseBilibili, SH.ServiceGameAccountBilibiliNotSupported);

        string? registrySdk = RegistryInterop.Get(schemeType);
        return string.IsNullOrEmpty(registrySdk) ? default : SingleGameAccountOrDefault(gameAccounts.Source.AsReadOnly(), registrySdk);
    }

    public bool SetCurrentGameAccount(GameAccount account)
    {
        return RegistryInterop.Set(account);
    }

    public async ValueTask ModifyGameAccountAsync(GameAccount gameAccount, Func<string, Task<ValueResult<bool, string?>>> providerNameCallback)
    {
        if (await providerNameCallback(gameAccount.Name).ConfigureAwait(false) is not (true, { } name))
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        gameAccount.UpdateName(name);

        await taskContext.SwitchToBackgroundAsync();
        gameAccountRepository.UpdateGameAccount(gameAccount);
    }

    public async ValueTask RemoveGameAccountAsync(GameAccount gameAccount)
    {
        ArgumentNullException.ThrowIfNull(gameAccounts);

        await taskContext.SwitchToMainThreadAsync();
        gameAccounts.Remove(gameAccount);

        await taskContext.SwitchToBackgroundAsync();
        gameAccountRepository.RemoveGameAccountById(gameAccount.InnerId);
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