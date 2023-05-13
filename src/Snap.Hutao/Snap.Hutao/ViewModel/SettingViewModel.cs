// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

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
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Locator;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.View.Dialog;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 设置视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class SettingViewModel : Abstraction.ViewModel
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly AppDbContext appDbContext;
    private readonly IGameService gameService;
    private readonly ILogger<SettingViewModel> logger;
    private readonly AppOptions options;
    private readonly HutaoOptions hutaoOptions;
    private readonly HutaoUserOptions hutaoUserOptions;
    private readonly ExperimentalFeaturesViewModel experimental;

    private NameValue<BackdropType>? selectedBackdropType;
    private NameValue<string>? selectedCulture;

    /// <summary>
    /// 应用程序设置
    /// </summary>
    public AppOptions Options { get => options; }

    /// <summary>
    /// 胡桃选项
    /// </summary>
    public HutaoOptions HutaoOptions { get => hutaoOptions; }

    /// <summary>
    /// 胡桃用户选项
    /// </summary>
    public HutaoUserOptions UserOptions { get => hutaoUserOptions; }

    /// <summary>
    /// 是否切换到 星穹铁道 工具箱
    /// </summary>
    public bool IsSwitchToStarRailTool { get => StaticResource.IsSwitchToStarRailTool(); }

    /// <summary>
    /// 选中的背景类型
    /// </summary>
    public NameValue<BackdropType>? SelectedBackdropType
    {
        get => selectedBackdropType ??= Options.BackdropTypes.Single(t => t.Value == Options.BackdropType);
        set
        {
            if (SetProperty(ref selectedBackdropType, value) && value != null)
            {
                Options.BackdropType = value.Value;
            }
        }
    }

    /// <summary>
    /// 选中的语言
    /// </summary>
    public NameValue<string>? SelectedCulture
    {
        get => selectedCulture ??= Options.Cultures.FirstOrDefault(c => c.Value == Options.CurrentCulture.Name);
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
    public ExperimentalFeaturesViewModel Experimental { get => experimental; }

    /// <summary>
    /// 是否提权
    /// </summary>
    [SuppressMessage("", "CA1822")]
    public bool IsElevated
    {
        get => Activation.GetElevated();
    }

    /// <summary>
    /// 切换到星穹铁道工具或者箱原神工具箱
    /// </summary>
    public string SwitchToStarRailToolsOrGenshinToolsHeaderInfo
    {
        get
        {
            return gameService.IsSwitchToStarRailTools switch
            {
                false => SH.ViewPageSettingSwitchBetweenStarRailOrGenshinToolHeaderStarRail,
                true => SH.ViewPageSettingSwitchBetweenStarRailOrGenshinToolHeaderGenshin,
            };
        }
    }

    /// <summary>
    /// 添加自启动的头信息
    /// 添加自启动 / 取消自启动
    /// </summary>
    public string IncludeInSelfStartHeaderInfo
    {
        get
        {
            return Activation.IsIncludedInSelfStart() switch
            {
                false => SH.ViewPageSettingIncludeInSelfStartHeader,
                true => SH.ViewPageSettingExcludeInSelfStartHeader,
            };
        }
    }

    /// <inheritdoc/>
    protected override Task OpenUIAsync()
    {
        return Task.CompletedTask;
    }

    [Command("SetGamePathCommand")]
    private async Task SetGamePathAsync()
    {
        IGameLocator locator = serviceProvider.GetRequiredService<IEnumerable<IGameLocator>>().Pick(nameof(ManualGameLocator));

        (bool isOk, string path) = await locator.LocateGamePathAsync(gameService.IsSwitchToStarRailTools).ConfigureAwait(false);
        if (isOk && !gameService.IsSwitchToStarRailTools)
        {
            await taskContext.SwitchToMainThreadAsync();
            Options.GamePath = path;
        }
        else if (isOk && gameService.IsSwitchToStarRailTools)
        {
            await taskContext.SwitchToMainThreadAsync();
            Options.StarRailGamePath = path;
        }
    }

    [Command("DeleteGameWebCacheCommand")]
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

    [Command("ShowSignInWebViewDialogCommand")]
    private async Task ShowSignInWebViewDialogAsync()
    {
        // ContentDialog must be created by main thread.
        await taskContext.SwitchToMainThreadAsync();
        SignInWebViewDialog dialog = serviceProvider.CreateInstance<SignInWebViewDialog>();
        await dialog.ShowAsync();
    }

    [Command("UpdateCheckCommand")]
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

    [Command("SetDataFolderCommand")]
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

    [Command("ResetStaticResourceCommand")]
    private void ResetStaticResource()
    {
        StaticResource.UnfulfillAllContracts();
        AppInstance.Restart(string.Empty);
    }

    [Command("CopyDeviceIdCommand")]
    private void CopyDeviceId()
    {
        IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        try
        {
            serviceProvider.GetRequiredService<IClipboardInterop>().SetText(HutaoOptions.DeviceId);
            infoBarService.Success(SH.ViewModelSettingCopyDeviceIdSuccess);
        }
        catch (COMException ex)
        {
            infoBarService.Error(ex);
        }
    }

    [Command("NavigateToHutaoPassportCommand")]
    private void NavigateToHutaoPassport()
    {
        serviceProvider.GetRequiredService<INavigationService>().Navigate<View.Page.HutaoPassportPage>(INavigationAwaiter.Default);
    }
}