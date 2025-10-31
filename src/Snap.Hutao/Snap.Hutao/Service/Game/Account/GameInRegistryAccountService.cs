// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game.Account;

[Service(ServiceLifetime.Singleton, typeof(IGameInRegistryAccountService))]
internal sealed partial class GameInRegistryAccountService : IGameInRegistryAccountService
{
    private readonly IGameAccountRepository gameAccountRepository;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    private readonly AsyncLock gameAccountLock = new();
    private IAdvancedCollectionView<GameAccount>? gameAccounts;

    [GeneratedConstructor]
    public partial GameInRegistryAccountService(IServiceProvider serviceProvider);

    public async ValueTask<IAdvancedCollectionView<GameAccount>> GetGameAccountCollectionAsync()
    {
        using (await gameAccountLock.LockAsync().ConfigureAwait(false))
        {
            if (gameAccounts is null)
            {
                await taskContext.SwitchToBackgroundAsync();
                gameAccounts = gameAccountRepository.GetGameAccountCollection().AsAdvancedCollectionView();

                string macAddress = RegistryAccountDataListDecoder.GetMacAddress();
                foreach (GameAccount account in gameAccounts.Source)
                {
                    account.IsExpired = !string.Equals(account.MacAddress, macAddress, StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        return gameAccounts;
    }

    public async ValueTask<GameAccount?> DetectCurrentGameAccountAsync(SchemeType schemeType, Func<string, Task<ValueResult<bool, string?>>> providerNameCallback)
    {
        ArgumentNullException.ThrowIfNull(gameAccounts);
        HutaoException.NotSupportedIf(schemeType is SchemeType.ChineseBilibili, SH.ServiceGameAccountBilibiliNotSupported);

        if (!RegistryInterop.TryGet(schemeType, out string? registrySdk, out string? macAddress, out DataWrapper<ImmutableArray<AccountInformation>>? info))
        {
            messenger.Send(InfoBarMessage.Warning(SH.ServiceGameAccountRegistryTryGetFailed));
            return default;
        }

        if (info is null || info.Data.Length < 1)
        {
            messenger.Send(InfoBarMessage.Warning(SH.ServiceGameAccountRegistryNoAccountDetected));
            return default;
        }

        if (info.Data.Length > 1)
        {
            messenger.Send(InfoBarMessage.Warning(SH.ServiceGameAccountRegistryMultipleAccountDetected));
            return default;
        }

        if (string.IsNullOrEmpty(registrySdk))
        {
            return default;
        }

        if (SingleGameAccountMatchAdlOrDefault(gameAccounts.Source.AsReadOnly(), registrySdk) is { } account)
        {
            return account;
        }

        AccountInformation? userInfo = info?.Data.SingleOrDefault();
        string? mid = userInfo?.Mid;
        string suggestedName = $"{userInfo?.Uid} | {mid}";
        if (await providerNameCallback(suggestedName).ConfigureAwait(false) is not (true, { } name))
        {
            return default;
        }

        if (!string.IsNullOrEmpty(mid) && SingleGameAccountMatchMidOrDefault(gameAccounts.Source.AsReadOnly(), mid) is { } existingAccount)
        {
            existingAccount.MihoyoSDK = registrySdk;
            existingAccount.MacAddress = macAddress;

            await taskContext.SwitchToMainThreadAsync();
            existingAccount.UpdateName(name);

            await taskContext.SwitchToBackgroundAsync();
            gameAccountRepository.UpdateGameAccount(existingAccount);

            return existingAccount;
        }

        if (gameAccounts.Source.Any(a => a.Name == name))
        {
            messenger.Send(InfoBarMessage.Warning(SH.FormatServiceGameAccountDetectInputNameAlreadyExists(name)));
            return default;
        }

        account = GameAccount.Create(schemeType, name, registrySdk, userInfo?.Mid, macAddress);

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

        return RegistryInterop.TryGet(schemeType, out string? registrySdk, out _, out _)
            ? SingleGameAccountMatchAdlOrDefault(gameAccounts.Source.AsReadOnly(), registrySdk)
            : default;
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

    private static GameAccount? SingleGameAccountMatchAdlOrDefault(IReadOnlyCollection<GameAccount> gameAccounts, string registrySdk)
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

    private static GameAccount? SingleGameAccountMatchMidOrDefault(IReadOnlyCollection<GameAccount> gameAccounts, string mid)
    {
        try
        {
            return gameAccounts.SingleOrDefault(a => a.Mid == mid);
        }
        catch (InvalidOperationException ex)
        {
            throw HutaoException.Throw(SH.ServiceGameDetectGameAccountMultiMatched, ex);
        }
    }
}