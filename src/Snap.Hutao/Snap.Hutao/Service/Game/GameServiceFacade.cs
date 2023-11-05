// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game.Account;
using Snap.Hutao.Service.Game.Configuration;
using Snap.Hutao.Service.Game.Package;
using Snap.Hutao.Service.Game.Process;
using Snap.Hutao.Service.Game.Scheme;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 游戏服务
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameServiceFacade))]
internal sealed partial class GameServiceFacade : IGameServiceFacade
{
    private readonly IGameChannelOptionsService gameChannelOptionsService;
    private readonly IGameAccountService gameAccountService;
    private readonly IGameProcessService gameProcessService;
    private readonly IGamePackageService gamePackageService;
    private readonly IGamePathService gamePathService;

    /// <inheritdoc/>
    public ObservableCollection<GameAccount> GameAccountCollection
    {
        get => gameAccountService.GameAccountCollection;
    }

    /// <inheritdoc/>
    public ValueTask<ValueResult<bool, string>> GetGamePathAsync()
    {
        return gamePathService.SilentGetGamePathAsync();
    }

    /// <inheritdoc/>
    public ChannelOptions GetChannelOptions()
    {
        return gameChannelOptionsService.GetChannelOptions();
    }

    /// <inheritdoc/>
    public bool SetChannelOptions(LaunchScheme scheme)
    {
        return gameChannelOptionsService.SetChannelOptions(scheme);
    }

    /// <inheritdoc/>
    public ValueTask<GameAccount?> DetectGameAccountAsync()
    {
        return gameAccountService.DetectGameAccountAsync();
    }

    /// <inheritdoc/>
    public GameAccount? DetectCurrentGameAccount()
    {
        return gameAccountService.DetectCurrentGameAccount();
    }

    /// <inheritdoc/>
    public bool SetGameAccount(GameAccount account)
    {
        return gameAccountService.SetGameAccount(account);
    }

    /// <inheritdoc/>
    public void AttachGameAccountToUid(GameAccount gameAccount, string uid)
    {
        gameAccountService.AttachGameAccountToUid(gameAccount, uid);
    }

    /// <inheritdoc/>
    public ValueTask ModifyGameAccountAsync(GameAccount gameAccount)
    {
        return gameAccountService.ModifyGameAccountAsync(gameAccount);
    }

    /// <inheritdoc/>
    public ValueTask RemoveGameAccountAsync(GameAccount gameAccount)
    {
        return gameAccountService.RemoveGameAccountAsync(gameAccount);
    }

    /// <inheritdoc/>
    public bool IsGameRunning()
    {
        return gameProcessService.IsGameRunning();
    }

    /// <inheritdoc/>
    public ValueTask LaunchAsync(IProgress<LaunchStatus> progress)
    {
        return gameProcessService.LaunchAsync(progress);
    }

    /// <inheritdoc/>
    public ValueTask<bool> EnsureGameResourceAsync(LaunchScheme launchScheme, IProgress<PackageReplaceStatus> progress)
    {
        return gamePackageService.EnsureGameResourceAsync(launchScheme, progress);
    }
}