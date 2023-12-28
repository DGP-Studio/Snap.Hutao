// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Configuration;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;
using System.Collections.ObjectModel;
using Windows.Win32.Foundation;

namespace Snap.Hutao.ViewModel.Game;

/// <summary>
/// 简化的启动游戏视图模型
/// </summary>
[Injection(InjectAs.Transient)]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class LaunchGameViewModelSlim : Abstraction.ViewModelSlim<View.Page.LaunchGamePage>
{
    private readonly IGameServiceFacade gameService;
    private readonly ITaskContext taskContext;
    private readonly IInfoBarService infoBarService;

    private ObservableCollection<GameAccount>? gameAccounts;
    private GameAccount? selectedGameAccount;

    /// <summary>
    /// 游戏账号集合
    /// </summary>
    public ObservableCollection<GameAccount>? GameAccounts { get => gameAccounts; set => SetProperty(ref gameAccounts, value); }

    /// <summary>
    /// 选中的账号
    /// </summary>
    public GameAccount? SelectedGameAccount { get => selectedGameAccount; set => SetProperty(ref selectedGameAccount, value); }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        ObservableCollection<GameAccount> accounts = gameService.GameAccountCollection;
        await taskContext.SwitchToMainThreadAsync();
        GameAccounts = accounts;

        ChannelOptions options = gameService.GetChannelOptions();
        LaunchScheme? scheme = default;
        if (string.IsNullOrEmpty(options.ConfigFilePath))
        {
            try
            {
                scheme = KnownLaunchSchemes.Get().Single(scheme => scheme.Equals(options));
            }
            catch (InvalidOperationException)
            {
                if (!IgnoredInvalidChannelOptions.Contains(options))
                {
                    // 后台收集
                    throw ThrowHelper.NotSupported($"不支持的 MultiChannel: {options}");
                }
            }
        }
        else
        {
            infoBarService.Warning(SH.FormatViewModelLaunchGameMultiChannelReadFail(options.ConfigFilePath));
        }

        try
        {
            if (scheme is not null)
            {
                // Try set to the current account.
                SelectedGameAccount ??= gameService.DetectCurrentGameAccount(scheme);
            }
        }
        catch (UserdataCorruptedException ex)
        {
            infoBarService.Error(ex);
        }
    }

    [Command("LaunchCommand")]
    private async Task LaunchAsync()
    {
        IInfoBarService infoBarService = ServiceProvider.GetRequiredService<IInfoBarService>();

        try
        {
            if (SelectedGameAccount is not null)
            {
                if (!gameService.SetGameAccount(SelectedGameAccount))
                {
                    infoBarService.Warning(SH.ViewModelLaunchGameSwitchGameAccountFail);
                    return;
                }
            }

            await gameService.LaunchAsync(new Progress<LaunchStatus>()).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (ex is Win32Exception win32Exception && win32Exception.HResult == HRESULT.E_FAIL)
            {
                // User canceled the operation. ignore
                return;
            }

            infoBarService.Error(ex);
        }
    }
}