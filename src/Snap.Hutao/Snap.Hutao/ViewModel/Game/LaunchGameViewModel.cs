// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using System.Collections.ObjectModel;
using System.IO;

namespace Snap.Hutao.ViewModel.Game;

/// <summary>
/// 启动游戏视图模型
/// </summary>
[HighQuality]
[Injection(InjectAs.Scoped)]
internal sealed class LaunchGameViewModel : Abstraction.ViewModel
{
    /// <summary>
    /// 启动游戏目标 Uid
    /// </summary>
    public const string DesiredUid = nameof(DesiredUid);

    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly IGameService gameService;
    private readonly IMemoryCache memoryCache;

    private readonly List<LaunchScheme> knownSchemes = LaunchScheme.KnownSchemes.ToList();

    private LaunchScheme? selectedScheme;
    private ObservableCollection<GameAccount>? gameAccounts;
    private GameAccount? selectedGameAccount;
    private GameResource? gameResource;

    /// <summary>
    /// 构造一个新的启动游戏视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public LaunchGameViewModel(IServiceProvider serviceProvider)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        gameService = serviceProvider.GetRequiredService<IGameService>();
        memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        Options = serviceProvider.GetRequiredService<LaunchOptions>();
        AppOptions = serviceProvider.GetRequiredService<AppOptions>();
        this.serviceProvider = serviceProvider;

        LaunchCommand = new AsyncRelayCommand(LaunchAsync, AsyncRelayCommandOptions.AllowConcurrentExecutions);
        DetectGameAccountCommand = new AsyncRelayCommand(DetectGameAccountAsync);
        ModifyGameAccountCommand = new AsyncRelayCommand<GameAccount>(ModifyGameAccountAsync);
        RemoveGameAccountCommand = new AsyncRelayCommand<GameAccount>(RemoveGameAccountAsync);
        AttachGameAccountCommand = new RelayCommand<GameAccount>(AttachGameAccountToCurrentUserGameRole);
        OpenScreenshotFolderCommand = new AsyncRelayCommand(OpenScreenshotFolderAsync);
    }

    /// <summary>
    /// 已知的服务器方案
    /// </summary>
    public List<LaunchScheme> KnownSchemes { get => knownSchemes; }

    /// <summary>
    /// 当前选择的服务器方案
    /// </summary>
    public LaunchScheme? SelectedScheme
    {
        get => selectedScheme; set
        {
            if (SetProperty(ref selectedScheme, value))
            {
                if (value != null)
                {
                    UpdateGameResourceAsync(value).SafeForget();
                }
            }
        }
    }

    /// <summary>
    /// 游戏账号集合
    /// </summary>
    public ObservableCollection<GameAccount>? GameAccounts { get => gameAccounts; set => SetProperty(ref gameAccounts, value); }

    /// <summary>
    /// 选中的账号
    /// </summary>
    public GameAccount? SelectedGameAccount { get => selectedGameAccount; set => SetProperty(ref selectedGameAccount, value); }

    /// <summary>
    /// 启动选项
    /// </summary>
    public LaunchOptions Options { get; }

    /// <summary>
    /// 应用选项
    /// </summary>
    public AppOptions AppOptions { get; }

    /// <summary>
    /// 游戏资源
    /// </summary>
    public GameResource? GameResource { get => gameResource; set => SetProperty(ref gameResource, value); }

    /// <summary>
    /// 是否提权
    /// </summary>
    [SuppressMessage("", "CA1822")]
    public bool IsElevated { get => Activation.GetElevated(); }

    /// <summary>
    /// 启动游戏命令
    /// </summary>
    public ICommand LaunchCommand { get; }

    /// <summary>
    /// 检测游戏账号命令
    /// </summary>
    public ICommand DetectGameAccountCommand { get; }

    /// <summary>
    /// 修改游戏账号命令
    /// </summary>
    public ICommand ModifyGameAccountCommand { get; }

    /// <summary>
    /// 删除游戏账号命令
    /// </summary>
    public ICommand RemoveGameAccountCommand { get; }

    /// <summary>
    /// 绑定到Uid命令
    /// </summary>
    public ICommand AttachGameAccountCommand { get; }

    /// <summary>
    /// 打开截图文件夹命令
    /// </summary>
    public ICommand OpenScreenshotFolderCommand { get; }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();

        if (File.Exists(AppOptions.GamePath))
        {
            try
            {
                using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
                {
                    MultiChannel multi = gameService.GetMultiChannel();
                    if (string.IsNullOrEmpty(multi.ConfigFilePath))
                    {
                        try
                        {
                            SelectedScheme = KnownSchemes.Single(scheme => scheme.MultiChannelEqual(multi));
                        }
                        catch (InvalidOperationException)
                        {
                            // 后台收集用
                            throw new NotSupportedException($"不支持的 MultiChannel: [ChannelType:{multi.Channel}] [SubChannel:{multi.SubChannel}]");
                        }
                    }
                    else
                    {
                        infoBarService.Warning(string.Format(SH.ViewModelLaunchGameMultiChannelReadFail, multi.ConfigFilePath));
                    }

                    ObservableCollection<GameAccount> accounts = gameService.GameAccountCollection;

                    await taskContext.SwitchToMainThreadAsync();
                    GameAccounts = accounts;

                    // Sync uid
                    if (memoryCache.TryRemove(DesiredUid, out object? value) && value is string uid)
                    {
                        SelectedGameAccount = GameAccounts.FirstOrDefault(g => g.AttachUid == uid);
                    }

                    // Try set to the current account.
                    SelectedGameAccount ??= gameService.DetectCurrentGameAccount();
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
        else
        {
            infoBarService.Warning(SH.ViewModelLaunchGamePathInvalid);
            await taskContext.SwitchToMainThreadAsync();
            await serviceProvider.GetRequiredService<INavigationService>()
                .NavigateAsync<View.Page.SettingPage>(INavigationAwaiter.Default, true)
                .ConfigureAwait(false);
        }
    }

    private async Task UpdateGameResourceAsync(LaunchScheme scheme)
    {
        await taskContext.SwitchToBackgroundAsync();
        Web.Response.Response<GameResource> response = await serviceProvider
            .GetRequiredService<ResourceClient>()
            .GetResourceAsync(scheme)
            .ConfigureAwait(false);

        if (response.IsOk())
        {
            await taskContext.SwitchToMainThreadAsync();
            GameResource = response.Data;
        }
    }

    private async Task LaunchAsync()
    {
        IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();

        if (SelectedScheme != null)
        {
            try
            {
                if (gameService.SetMultiChannel(SelectedScheme))
                {
                    // Channel changed, we need to change local file.
                    await taskContext.SwitchToMainThreadAsync();
                    LaunchGamePackageConvertDialog dialog = serviceProvider.CreateInstance<LaunchGamePackageConvertDialog>();
                    Progress<Service.Game.Package.PackageReplaceStatus> progress = new(state => dialog.State = state.Clone());
                    using (await dialog.BlockAsync(taskContext).ConfigureAwait(false))
                    {
                        if (!await gameService.EnsureGameResourceAsync(SelectedScheme, progress).ConfigureAwait(false))
                        {
                            infoBarService.Warning(SH.ViewModelLaunchGameEnsureGameResourceFail);
                            return;
                        }
                    }
                }

                if (SelectedGameAccount != null)
                {
                    if (!gameService.SetGameAccount(SelectedGameAccount))
                    {
                        infoBarService.Warning(SH.ViewModelLaunchGameSwitchGameAccountFail);
                        return;
                    }
                }

                await gameService.LaunchAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                infoBarService.Error(ex);
            }
        }
        else
        {
            infoBarService.Error(SH.ViewModelLaunchGameSchemeNotSelected);
        }
    }

    private async Task DetectGameAccountAsync()
    {
        try
        {
            await gameService.DetectGameAccountAsync().ConfigureAwait(false);
        }
        catch (UserdataCorruptedException ex)
        {
            serviceProvider.GetRequiredService<IInfoBarService>().Error(ex);
        }
    }

    private void AttachGameAccountToCurrentUserGameRole(GameAccount? gameAccount)
    {
        if (gameAccount != null)
        {
            IUserService userService = serviceProvider.GetRequiredService<IUserService>();
            IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();

            if (userService.Current?.SelectedUserGameRole is UserGameRole role)
            {
                gameService.AttachGameAccountToUid(gameAccount, role.GameUid);
            }
            else
            {
                infoBarService.Warning(SH.MustSelectUserAndUid);
            }
        }
    }

    private async Task ModifyGameAccountAsync(GameAccount? gameAccount)
    {
        if (gameAccount != null)
        {
            await gameService.ModifyGameAccountAsync(gameAccount).ConfigureAwait(false);
        }
    }

    private async Task RemoveGameAccountAsync(GameAccount? gameAccount)
    {
        if (gameAccount != null)
        {
            await gameService.RemoveGameAccountAsync(gameAccount).ConfigureAwait(false);
        }
    }

    private async Task OpenScreenshotFolderAsync()
    {
        string game = serviceProvider.GetRequiredService<AppOptions>().GamePath;
        string screenshot = Path.Combine(Path.GetDirectoryName(game)!, "ScreenShot");
        if (Directory.Exists(screenshot))
        {
            await Windows.System.Launcher.LaunchFolderPathAsync(screenshot);
        }
    }
}