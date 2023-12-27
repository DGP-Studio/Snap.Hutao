// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core;
using Snap.Hutao.Core.IO.DataTransfer;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Core.Shell;
using Snap.Hutao.Core.Windowing;
using Snap.Hutao.Core.Windowing.HotKey;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Model;
using Snap.Hutao.Service;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Locator;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.ViewModel.Guide;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Response;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Windows.System;

namespace Snap.Hutao.ViewModel.Setting;

/// <summary>
/// 设置视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class SettingViewModel : Abstraction.ViewModel
{
    private readonly HomeCardOptions homeCardOptions = new();

    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly HutaoInfrastructureClient hutaoInfrastructureClient;
    private readonly HutaoPassportViewModel hutaoPassportViewModel;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IGameLocatorFactory gameLocatorFactory;
    private readonly INavigationService navigationService;
    private readonly IClipboardProvider clipboardInterop;
    private readonly IShellLinkInterop shellLinkInterop;
    private readonly HutaoUserOptions hutaoUserOptions;
    private readonly IInfoBarService infoBarService;
    private readonly RuntimeOptions runtimeOptions;
    private readonly LaunchOptions launchOptions;
    private readonly HotKeyOptions hotKeyOptions;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;

    private NameValue<BackdropType>? selectedBackdropType;
    private NameValue<CultureInfo>? selectedCulture;
    private NameValue<Region>? selectedRegion;
    private IPInformation? ipInformation;
    private FolderViewModel? cacheFolderView;
    private FolderViewModel? dataFolderView;

    public AppOptions AppOptions { get => appOptions; }

    public RuntimeOptions HutaoOptions { get => runtimeOptions; }

    public HutaoUserOptions UserOptions { get => hutaoUserOptions; }

    public HomeCardOptions HomeCardOptions { get => homeCardOptions; }

    public HotKeyOptions HotKeyOptions { get => hotKeyOptions; }

    public LaunchOptions LaunchOptions { get => launchOptions; }

    public HutaoPassportViewModel Passport { get => hutaoPassportViewModel; }

    public NameValue<BackdropType>? SelectedBackdropType
    {
        get => selectedBackdropType ??= AppOptions.BackdropTypes.Single(t => t.Value == AppOptions.BackdropType);
        set
        {
            if (SetProperty(ref selectedBackdropType, value) && value is not null)
            {
                AppOptions.BackdropType = value.Value;
            }
        }
    }

    public NameValue<CultureInfo>? SelectedCulture
    {
        get => selectedCulture ??= AppOptions.GetCurrentCultureForSelectionOrDefault();
        set
        {
            if (SetProperty(ref selectedCulture, value) && value is not null)
            {
                AppOptions.CurrentCulture = value.Value;
                AppInstance.Restart(string.Empty);
            }
        }
    }

    public NameValue<Region>? SelectedRegion
    {
        get => selectedRegion ??= AppOptions.GetCurrentRegionForSelectionOrDefault();
        set
        {
            if (SetProperty(ref selectedRegion, value) && value is not null)
            {
                AppOptions.Region = value.Value;
            }
        }
    }

    public FolderViewModel? CacheFolderView { get => cacheFolderView; set => SetProperty(ref cacheFolderView, value); }

    public FolderViewModel? DataFolderView { get => dataFolderView; set => SetProperty(ref dataFolderView, value); }

    public IPInformation? IPInformation { get => ipInformation; private set => SetProperty(ref ipInformation, value); }

    [SuppressMessage("", "CA1822")]
    public bool IsAllocConsoleDebugModeEnabled
    {
        get => LocalSetting.Get(SettingKeys.IsAllocConsoleDebugModeEnabled, false);
        set => LocalSetting.Set(SettingKeys.IsAllocConsoleDebugModeEnabled, value);
    }

    protected override async ValueTask<bool> InitializeUIAsync()
    {
        CacheFolderView = new(taskContext, runtimeOptions.LocalCache);
        DataFolderView = new(taskContext, runtimeOptions.DataFolder);

        Response<IPInformation> resp = await hutaoInfrastructureClient.GetIPInformationAsync().ConfigureAwait(false);
        IPInformation info;

        if (resp.IsOk())
        {
            info = resp.Data;
        }
        else
        {
            info = IPInformation.Default;
        }

        await taskContext.SwitchToMainThreadAsync();
        IPInformation = info;

        return true;
    }

    [Command("ResetStaticResourceCommand")]
    private static void ResetStaticResource()
    {
        StaticResource.FailAll();
        UnsafeLocalSetting.Set(SettingKeys.Major1Minor7Revision0GuideState, GuideState.StaticResourceBegin);
        AppInstance.Restart(string.Empty);
    }

    [Command("StoreReviewCommand")]
    private static async Task StoreReviewAsync()
    {
        await Launcher.LaunchUriAsync(new("ms-windows-store://review/?ProductId=9PH4NXJ2JN52"));
    }

    [Command("UpdateCheckCommand")]
    private static async Task CheckUpdateAsync()
    {
        await Launcher.LaunchUriAsync(new("ms-windows-store://pdp/?productid=9PH4NXJ2JN52"));
    }

    [Command("DeleteGameWebCacheCommand")]
    private void DeleteGameWebCache()
    {
        string gamePath = launchOptions.GamePath;

        if (!string.IsNullOrEmpty(gamePath))
        {
            string cacheFilePath = GachaLogQueryWebCacheProvider.GetCacheFile(gamePath);
            string? cacheFolder = Path.GetDirectoryName(cacheFilePath);

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
                infoBarService.Warning(SH.FormatViewModelSettingClearWebCachePathInvalid(cacheFolder));
            }
        }
    }

    [Command("OpenTestPageCommand")]
    private async Task OpenTestPageAsync()
    {
        await navigationService
            .NavigateAsync<View.Page.TestPage>(INavigationAwaiter.Default)
            .ConfigureAwait(false);
    }

    [Command("SetDataFolderCommand")]
    private void SetDataFolder()
    {
        (bool isOk, string folder) = fileSystemPickerInteraction.PickFolder();

        if (isOk)
        {
            LocalSetting.Set(SettingKeys.DataFolderPath, folder);
            infoBarService.Success(SH.ViewModelSettingSetDataFolderSuccess);
        }
    }

    [Command("CopyDeviceIdCommand")]
    private void CopyDeviceId()
    {
        try
        {
            clipboardInterop.SetText(HutaoOptions.DeviceId);
            infoBarService.Success(SH.ViewModelSettingCopyDeviceIdSuccess);
        }
        catch (COMException ex)
        {
            infoBarService.Error(ex);
        }
    }

    [Command("DeleteUsersCommand")]
    private async Task DangerousDeleteUsersAsync()
    {
        if (userService is IUserServiceUnsafe @unsafe)
        {
            ContentDialogResult result = await contentDialogFactory
                .CreateForConfirmCancelAsync(SH.ViewDialogSettingDeleteUserDataTitle, SH.ViewDialogSettingDeleteUserDataContent)
                .ConfigureAwait(false);

            if (result is ContentDialogResult.Primary)
            {
                await @unsafe.UnsafeRemoveUsersAsync().ConfigureAwait(false);
                AppInstance.Restart(string.Empty);
            }
        }
    }

    [Command("ConfigureGeetestUrlCommand")]
    private async Task ConfigureGeetestUrlAsync()
    {
        GeetestCustomUrlDialog dialog = await contentDialogFactory.CreateInstanceAsync<GeetestCustomUrlDialog>().ConfigureAwait(false);
        (bool isOk, string template) = await dialog.GetUrlAsync().ConfigureAwait(false);

        if (isOk)
        {
            await taskContext.SwitchToMainThreadAsync();
            appOptions.GeetestCustomCompositeUrl = template;
            infoBarService.Success(SH.ViewModelSettingGeetestCustomUrlSucceed);
        }
    }

    [Command("CreateDesktopShortcutCommand")]
    private async Task CreateDesktopShortcutForElevatedLaunchAsync()
    {
        if (await shellLinkInterop.TryCreateDesktopShoutcutForElevatedLaunchAsync().ConfigureAwait(false))
        {
            infoBarService.Information(SH.ViewModelSettingActionComplete);
        }
        else
        {
            infoBarService.Warning(SH.ViewModelSettingCreateDesktopShortcutFailed);
        }
    }

    [Command("RestartAsElevatedCommand")]
    private void RestartAsElevated()
    {
        Process.Start(new ProcessStartInfo()
        {
            FileName = $"shell:AppsFolder\\{runtimeOptions.FamilyName}!App",
            UseShellExecute = true,
            Verb = "runas",
        });

        Process.GetCurrentProcess().Kill();
    }
}