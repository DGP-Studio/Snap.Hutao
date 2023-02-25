// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Core.Windowing;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Locator;
using Snap.Hutao.View.Dialog;
using System.Globalization;
using System.IO;
using Windows.Storage.Pickers;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 设置视图模型
/// </summary>
[HighQuality]
[Injection(InjectAs.Scoped)]
internal sealed class SettingViewModel : Abstraction.ViewModel
{
    private readonly IServiceProvider serviceProvider;
    private readonly AppDbContext appDbContext;
    private readonly IGameService gameService;
    private readonly ILogger<SettingViewModel> logger;
    private readonly SettingEntry isEmptyHistoryWishVisibleEntry;
    private readonly SettingEntry selectedBackdropTypeEntry;
    private readonly List<NameValue<BackdropType>> backdropTypes = new()
    {
        new("Acrylic", BackdropType.Acrylic),
        new("Mica", BackdropType.Mica),
        new("MicaAlt", BackdropType.MicaAlt),
    };

    private readonly List<NameValue<string>> cultures = new()
    {
        ToNameValue(CultureInfo.CreateSpecificCulture("zh-CN")),
        ToNameValue(CultureInfo.CreateSpecificCulture("zh-TW")),
        ToNameValue(CultureInfo.CreateSpecificCulture("en-US")),
        ToNameValue(CultureInfo.CreateSpecificCulture("ko-KR")),
    };

    private bool isEmptyHistoryWishVisible;
    private string gamePath;
    private NameValue<BackdropType> selectedBackdropType;
    private NameValue<string>? selectedCulture;

    /// <summary>
    /// 构造一个新的设置视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public SettingViewModel(IServiceProvider serviceProvider)
    {
        appDbContext = serviceProvider.GetRequiredService<AppDbContext>();
        gameService = serviceProvider.GetRequiredService<IGameService>();
        logger = serviceProvider.GetRequiredService<ILogger<SettingViewModel>>();
        Experimental = serviceProvider.GetRequiredService<ExperimentalFeaturesViewModel>();
        this.serviceProvider = serviceProvider;

        isEmptyHistoryWishVisibleEntry = appDbContext.Settings.SingleOrAdd(SettingEntry.IsEmptyHistoryWishVisible, Core.StringLiterals.False);
        IsEmptyHistoryWishVisible = bool.Parse(isEmptyHistoryWishVisibleEntry.Value!);

        string? cultureName = appDbContext.Settings.SingleOrAdd(SettingEntry.Culture, CultureInfo.CurrentCulture.Name).Value;
        selectedCulture = cultures.FirstOrDefault(c => c.Value == cultureName);

        selectedBackdropTypeEntry = appDbContext.Settings.SingleOrAdd(SettingEntry.SystemBackdropType, BackdropType.Mica.ToString());
        BackdropType type = Enum.Parse<BackdropType>(selectedBackdropTypeEntry.Value!);

        // prevent unnecessary backdrop setting.
        selectedBackdropType = backdropTypes.Single(t => t.Value == type);
        OnPropertyChanged(nameof(SelectedBackdropType));

        GamePath = gameService.GetGamePathSkipLocator();

        SetGamePathCommand = new AsyncRelayCommand(SetGamePathAsync);
        UpdateCheckCommand = new AsyncRelayCommand(CheckUpdateAsync);
        DeleteGameWebCacheCommand = new RelayCommand(DeleteGameWebCache);
        ShowSignInWebViewDialogCommand = new AsyncRelayCommand(ShowSignInWebViewDialogAsync);
        SetDataFolderCommand = new AsyncRelayCommand(SetDataFolderAsync);
        ResetStaticResourceCommand = new RelayCommand(ResetStaticResource);
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
    public List<NameValue<BackdropType>> BackdropTypes { get => backdropTypes; }

    /// <summary>
    /// 选中的背景类型
    /// </summary>
    public NameValue<BackdropType> SelectedBackdropType
    {
        get => selectedBackdropType;
        [MemberNotNull(nameof(selectedBackdropType))]
        set
        {
            if (SetProperty(ref selectedBackdropType, value) && value != null)
            {
                selectedBackdropTypeEntry.Value = value.Value.ToString();
                appDbContext.Settings.UpdateAndSave(selectedBackdropTypeEntry);
                serviceProvider.GetRequiredService<IMessenger>().Send(new Message.BackdropTypeChangedMessage(value.Value));
            }
        }
    }

    /// <summary>
    /// 语言
    /// </summary>
    public List<NameValue<string>> Cultures { get => cultures; }

    /// <summary>
    /// 选中的语言
    /// </summary>
    public NameValue<string>? SelectedCulture
    {
        get => selectedCulture;
        set
        {
            if (SetProperty(ref selectedCulture, value))
            {
                SettingEntry entry = appDbContext.Settings.SingleOrAdd(SettingEntry.Culture, CultureInfo.CurrentCulture.Name);
                entry.Value = selectedCulture?.Value;
                appDbContext.Settings.UpdateAndSave(entry);
                AppInstance.Restart(string.Empty);
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
    public ICommand UpdateCheckCommand { get; }

    /// <summary>
    /// 删除游戏网页缓存命令
    /// </summary>
    public ICommand DeleteGameWebCacheCommand { get; }

    /// <summary>
    /// 签到对话框命令
    /// </summary>
    public ICommand ShowSignInWebViewDialogCommand { get; }

    /// <summary>
    /// 设置数据目录命令
    /// </summary>
    public ICommand SetDataFolderCommand { get; }

    /// <summary>
    /// 重置静态资源
    /// </summary>
    public ICommand ResetStaticResourceCommand { get; }

    private static NameValue<string> ToNameValue(CultureInfo info)
    {
        return new(info.NativeName, info.Name);
    }

    private async Task SetGamePathAsync()
    {
        IGameLocator locator = serviceProvider.GetRequiredService<IEnumerable<IGameLocator>>()
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
        IGameService gameService = serviceProvider.GetRequiredService<IGameService>();
        string gamePath = gameService.GetGamePathSkipLocator();

        if (!string.IsNullOrEmpty(gamePath))
        {
            string cacheFilePath = GachaLogQueryWebCacheProvider.GetCacheFile(gamePath);
            string cacheFolder = Path.GetDirectoryName(cacheFilePath)!;

            IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
            if (Directory.Exists(cacheFolder))
            {
                try
                {
                    Directory.Delete(cacheFolder, true);
                }
                catch (UnauthorizedAccessException)
                {
                    infoBarService.Warning(SH.ViewModelSettingClearWebCacheFail);
                    return;
                }

                infoBarService.Success(SH.ViewModelSettingClearWebCacheSuccess);
            }
            else
            {
                infoBarService.Warning(string.Format(SH.ViewModelSettingClearWebCachePathInvalid, cacheFolder));
            }
        }
    }

    private async Task ShowSignInWebViewDialogAsync()
    {
        // ContentDialog must be created by main thread.
        await ThreadHelper.SwitchToMainThreadAsync();
        await new SignInWebViewDialog().ShowAsync().AsTask().ConfigureAwait(false);
    }

    private async Task CheckUpdateAsync()
    {
#if DEBUG
        await serviceProvider
            .GetRequiredService<Service.Navigation.INavigationService>()
            .NavigateAsync<View.Page.TestPage>(Service.Navigation.INavigationAwaiter.Default)
            .ConfigureAwait(false);
#else
        await Windows.System.Launcher.LaunchUriAsync(new(@"ms-windows-store://pdp/?productid=9PH4NXJ2JN52"));
#endif
    }

    private async Task SetDataFolderAsync()
    {
        IPickerFactory pickerFactory = serviceProvider.GetRequiredService<IPickerFactory>();
        FolderPicker picker = pickerFactory.GetFolderPicker();
        (bool isOk, string folder) = await picker.TryPickSingleFolderAsync().ConfigureAwait(false);

        IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        if (isOk)
        {
            LocalSetting.Set(SettingKeys.DataFolderPath, folder);
            infoBarService.Success(SH.ViewModelSettingSetDataFolderSuccess);
        }
    }

    private void ResetStaticResource()
    {
        StaticResource.UnfulfillAllContracts();
        AppInstance.Restart(string.Empty);
    }
}