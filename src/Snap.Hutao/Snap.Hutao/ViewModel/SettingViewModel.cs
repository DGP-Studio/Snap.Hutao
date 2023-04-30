// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.DataTransfer;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Core.Windowing;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Locator;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.View.Dialog;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 设置视图模型
/// </summary>
[HighQuality]
[Injection(InjectAs.Scoped)]
internal sealed class SettingViewModel : Abstraction.ViewModel
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly AppDbContext appDbContext;
    private readonly IGameService gameService;
    private readonly ILogger<SettingViewModel> logger;

    private readonly List<NameValue<BackdropType>> backdropTypes = new()
    {
        new("Acrylic", BackdropType.Acrylic),
        new("Mica", BackdropType.Mica),
        new("MicaAlt", BackdropType.MicaAlt),
    };

    private readonly List<NameValue<string>> cultures = new()
    {
        ToNameValue(CultureInfo.GetCultureInfo("zh-Hans")),
        ToNameValue(CultureInfo.GetCultureInfo("zh-Hant")),
        ToNameValue(CultureInfo.GetCultureInfo("en")),
        ToNameValue(CultureInfo.GetCultureInfo("ko")),
    };

    private NameValue<BackdropType>? selectedBackdropType;
    private NameValue<string>? selectedCulture;

    /// <summary>
    /// 构造一个新的设置视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public SettingViewModel(IServiceProvider serviceProvider)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        appDbContext = serviceProvider.GetRequiredService<AppDbContext>();
        gameService = serviceProvider.GetRequiredService<IGameService>();
        logger = serviceProvider.GetRequiredService<ILogger<SettingViewModel>>();
        Experimental = serviceProvider.GetRequiredService<ExperimentalFeaturesViewModel>();
        Options = serviceProvider.GetRequiredService<AppOptions>();
        UserOptions = serviceProvider.GetRequiredService<HutaoUserOptions>();
        HutaoOptions = serviceProvider.GetRequiredService<HutaoOptions>();
        this.serviceProvider = serviceProvider;

        selectedCulture = cultures.FirstOrDefault(c => c.Value == Options.CurrentCulture.Name);
        selectedBackdropType = backdropTypes.Single(t => t.Value == Options.BackdropType);

        SetGamePathCommand = new AsyncRelayCommand(SetGamePathAsync);
        UpdateCheckCommand = new AsyncRelayCommand(CheckUpdateAsync);
        DeleteGameWebCacheCommand = new RelayCommand(DeleteGameWebCache);
        ShowSignInWebViewDialogCommand = new AsyncRelayCommand(ShowSignInWebViewDialogAsync);
        SetDataFolderCommand = new AsyncRelayCommand(SetDataFolderAsync);
        ResetStaticResourceCommand = new RelayCommand(ResetStaticResource);
        CopyDeviceIdCommand = new RelayCommand(CopyDeviceId);
        NavigateToHutaoPassportCommand = new RelayCommand(NavigateToHutaoPassport);
    }

    /// <summary>
    /// 应用程序设置
    /// </summary>
    public AppOptions Options { get; }

    /// <summary>
    /// 胡桃选项
    /// </summary>
    public HutaoOptions HutaoOptions { get; }

    /// <summary>
    /// 胡桃用户选项
    /// </summary>
    public HutaoUserOptions UserOptions { get; }

    /// <summary>
    /// 背景类型
    /// </summary>
    public List<NameValue<BackdropType>> BackdropTypes { get => backdropTypes; }

    /// <summary>
    /// 选中的背景类型
    /// </summary>
    public NameValue<BackdropType>? SelectedBackdropType
    {
        get => selectedBackdropType;
        set
        {
            if (SetProperty(ref selectedBackdropType, value) && value != null)
            {
                Options.BackdropType = value.Value;
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
                if (value != null)
                {
                    Options.CurrentCulture = CultureInfo.GetCultureInfo(value.Value);
                    AppInstance.Restart(string.Empty);
                }
            }
        }
    }

    /// <summary>
    /// 实验性功能
    /// </summary>
    public ExperimentalFeaturesViewModel Experimental { get; }

    /// <summary>
    /// 是否提权
    /// </summary>
    [SuppressMessage("", "CA1822")]
    public bool IsElevated
    {
        get => Activation.GetElevated();
    }

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

    /// <summary>
    /// 复制设备ID
    /// </summary>
    public ICommand CopyDeviceIdCommand { get; }

    /// <summary>
    /// 导航到胡桃通行证界面
    /// </summary>
    public ICommand NavigateToHutaoPassportCommand { get; }

    /// <inheritdoc/>
    protected override Task OpenUIAsync()
    {
        return Task.CompletedTask;
    }

    private static NameValue<string> ToNameValue(CultureInfo info)
    {
        return new(info.NativeName, info.Name);
    }

    private async Task SetGamePathAsync()
    {
        IGameLocator locator = serviceProvider.GetRequiredService<IEnumerable<IGameLocator>>().Pick(nameof(ManualGameLocator));

        (bool isOk, string path) = await locator.LocateGamePathAsync().ConfigureAwait(false);
        if (isOk)
        {
            await taskContext.SwitchToMainThreadAsync();
            Options.GamePath = path;
        }
    }

    private void DeleteGameWebCache()
    {
        string gamePath = Options.GamePath;

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
        await taskContext.SwitchToMainThreadAsync();
        SignInWebViewDialog dialog = serviceProvider.CreateInstance<SignInWebViewDialog>();
        await dialog.ShowAsync();
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
        (bool isOk, string folder) = await serviceProvider
            .GetRequiredService<IPickerFactory>()
            .GetFolderPicker()
            .TryPickSingleFolderAsync()
            .ConfigureAwait(false);

        if (isOk)
        {
            LocalSetting.Set(SettingKeys.DataFolderPath, folder);
            serviceProvider.GetRequiredService<IInfoBarService>().Success(SH.ViewModelSettingSetDataFolderSuccess);
        }
    }

    private void ResetStaticResource()
    {
        StaticResource.UnfulfillAllContracts();
        AppInstance.Restart(string.Empty);
    }

    private void CopyDeviceId()
    {
        IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();

        try
        {
            Clipboard.SetText(HutaoOptions.DeviceId);
            infoBarService.Success(SH.ViewModelSettingCopyDeviceIdSuccess);
        }
        catch (COMException ex)
        {
            infoBarService.Error(ex);
        }
    }

    private void NavigateToHutaoPassport()
    {
        serviceProvider.GetRequiredService<INavigationService>().Navigate<View.Page.HutaoPassportPage>(INavigationAwaiter.Default);
    }
}