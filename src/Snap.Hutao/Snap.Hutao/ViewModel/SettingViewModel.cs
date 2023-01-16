// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Windowing;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Locator;
using Snap.Hutao.View.Dialog;
using System.IO;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 设置视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class SettingViewModel : Abstraction.ViewModel
{
    private readonly AppDbContext appDbContext;
    private readonly IGameService gameService;
    private readonly ILogger<SettingViewModel> logger;
    private readonly SettingEntry isEmptyHistoryWishVisibleEntry;
    private readonly SettingEntry selectedBackdropTypeEntry;
    private readonly List<NamedValue<BackdropType>> backdropTypes = new()
    {
        new("亚克力", BackdropType.Acrylic),
        new("云母", BackdropType.Mica),
        new("变种云母", BackdropType.MicaAlt),
    };

    private bool isEmptyHistoryWishVisible;
    private string gamePath;
    private NamedValue<BackdropType> selectedBackdropType;

    /// <summary>
    /// 构造一个新的设置视图模型
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="gameService">游戏服务</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    /// <param name="experimental">实验性功能</param>
    /// <param name="logger">日志器</param>
    public SettingViewModel(
        AppDbContext appDbContext,
        IGameService gameService,
        IAsyncRelayCommandFactory asyncRelayCommandFactory,
        ExperimentalFeaturesViewModel experimental,
        ILogger<SettingViewModel> logger)
    {
        this.appDbContext = appDbContext;
        this.gameService = gameService;
        this.logger = logger;

        Experimental = experimental;

        isEmptyHistoryWishVisibleEntry = appDbContext.Settings.SingleOrAdd(SettingEntry.IsEmptyHistoryWishVisible, SettingEntryHelper.TrueString);
        IsEmptyHistoryWishVisible = bool.Parse(isEmptyHistoryWishVisibleEntry.Value!);

        selectedBackdropTypeEntry = appDbContext.Settings.SingleOrAdd(SettingEntry.SystemBackdropType, BackdropType.Mica.ToString());
        BackdropType type = Enum.Parse<BackdropType>(selectedBackdropTypeEntry.Value!);

        // prevent unnecessary backdrop setting.
        selectedBackdropType = backdropTypes.Single(t => t.Value == type);
        OnPropertyChanged(nameof(SelectedBackdropType));

        GamePath = gameService.GetGamePathSkipLocator();

        SetGamePathCommand = asyncRelayCommandFactory.Create(SetGamePathAsync);
        DebugExceptionCommand = asyncRelayCommandFactory.Create(DebugThrowExceptionAsync);
        DeleteGameWebCacheCommand = new RelayCommand(DeleteGameWebCache);
        ShowSignInWebViewDialogCommand = asyncRelayCommandFactory.Create(ShowSignInWebViewDialogAsync);
    }

    /// <summary>
    /// 版本
    /// </summary>
    [SuppressMessage("", "CA1822")]
    public string AppVersion
    {
        get => Core.CoreEnvironment.Version.ToString();
    }

    /// <summary>
    /// Webview2 版本
    /// </summary>
    [SuppressMessage("", "CA1822")]
    public string WebView2Version
    {
        get => Core.WebView2Helper.Version;
    }

    /// <summary>
    /// 设备Id
    /// </summary>
    [SuppressMessage("", "CA1822")]
    public string DeviceId
    {
        get => Core.CoreEnvironment.HutaoDeviceId;
    }

    /// <summary>
    /// 空的历史卡池是否可见
    /// </summary>
    public bool IsEmptyHistoryWishVisible
    {
        get => isEmptyHistoryWishVisible;
        set
        {
            if (SetProperty(ref isEmptyHistoryWishVisible, value))
            {
                isEmptyHistoryWishVisibleEntry.Value = value.ToString();
                appDbContext.Settings.UpdateAndSave(isEmptyHistoryWishVisibleEntry);
            }
        }
    }

    /// <summary>
    /// 游戏路径
    /// </summary>
    public string GamePath
    {
        get => gamePath;
        [MemberNotNull(nameof(gamePath))]
        set => SetProperty(ref gamePath, value);
    }

    /// <summary>
    /// 背景类型
    /// </summary>
    public List<NamedValue<BackdropType>> BackdropTypes { get => backdropTypes; }

    /// <summary>
    /// 选中的背景类型
    /// </summary>
    public NamedValue<BackdropType> SelectedBackdropType
    {
        get => selectedBackdropType;
        [MemberNotNull(nameof(selectedBackdropType))]
        set
        {
            if (SetProperty(ref selectedBackdropType, value))
            {
                selectedBackdropTypeEntry.Value = value.Value.ToString();
                appDbContext.Settings.UpdateAndSave(selectedBackdropTypeEntry);
                Ioc.Default.GetRequiredService<IMessenger>().Send(new Message.BackdropTypeChangedMessage(value.Value));
            }
        }
    }

    /// <summary>
    /// 实验性功能
    /// </summary>
    public ExperimentalFeaturesViewModel Experimental { get; }

    /// <summary>
    /// 设置游戏路径命令
    /// </summary>
    public ICommand SetGamePathCommand { get; }

    /// <summary>
    /// 调试异常命令
    /// </summary>
    public ICommand DebugExceptionCommand { get; }

    /// <summary>
    /// 删除游戏网页缓存命令
    /// </summary>
    public ICommand DeleteGameWebCacheCommand { get; }

    /// <summary>
    /// 签到对话框命令
    /// </summary>
    public ICommand ShowSignInWebViewDialogCommand { get; }

    private async Task SetGamePathAsync()
    {
        IGameLocator locator = Ioc.Default.GetRequiredService<IEnumerable<IGameLocator>>()
            .Single(l => l.Name == nameof(ManualGameLocator));

        (bool isOk, string path) = await locator.LocateGamePathAsync().ConfigureAwait(false);
        if (isOk)
        {
            gameService.OverwriteGamePath(path);
            await ThreadHelper.SwitchToMainThreadAsync();
            GamePath = path;
        }
    }

    private void DeleteGameWebCache()
    {
        IGameService gameService = Ioc.Default.GetRequiredService<IGameService>();
        string gamePath = gameService.GetGamePathSkipLocator();

        if (!string.IsNullOrEmpty(gamePath))
        {
            string cacheFilePath = GachaLogUrlWebCacheProvider.GetCacheFile(gamePath);
            string cacheFolder = Path.GetDirectoryName(cacheFilePath)!;

            IInfoBarService infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();
            if (Directory.Exists(cacheFolder))
            {
                try
                {
                    Directory.Delete(cacheFolder, true);
                }
                catch (UnauthorizedAccessException)
                {
                    infoBarService.Warning($"清除失败，文件目录权限不足，请使用管理员模式重试");
                    return;
                }

                infoBarService.Success("清除完成");
            }
            else
            {
                infoBarService.Warning($"清除失败，找不到目录：{cacheFolder}");
            }
        }
    }

    private async Task ShowSignInWebViewDialogAsync()
    {
        // ContentDialog must be created by main thread.
        await ThreadHelper.SwitchToMainThreadAsync();
        await new SignInWebViewDialog().ShowAsync().AsTask().ConfigureAwait(false);
    }

    private async Task DebugThrowExceptionAsync()
    {
#if DEBUG
        await Ioc.Default
            .GetRequiredService<Service.Navigation.INavigationService>()
            .NavigateAsync<View.Page.TestPage>(Service.Navigation.INavigationAwaiter.Default)
            .ConfigureAwait(false);
#else
        await Task.Yield();
#endif
    }
}