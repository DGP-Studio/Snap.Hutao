// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.DataTransfer;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Core.Windowing;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Service;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Service.Game.Locator;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.ViewModel.Guide;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Windows.System;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 设置视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class SettingViewModel : Abstraction.ViewModel
{
    private readonly IGameLocatorFactory gameLocatorFactory;
    private readonly IClipboardInterop clipboardInterop;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly INavigationService navigationService;
    private readonly IPickerFactory pickerFactory;
    private readonly IUserService userService;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;
    private readonly RuntimeOptions runtimeOptions;
    private readonly HutaoUserOptions hutaoUserOptions;

    private NameValue<BackdropType>? selectedBackdropType;
    private NameValue<string>? selectedCulture;

    /// <summary>
    /// 应用程序设置
    /// </summary>
    public AppOptions Options { get => appOptions; }

    /// <summary>
    /// 胡桃选项
    /// </summary>
    public RuntimeOptions HutaoOptions { get => runtimeOptions; }

    /// <summary>
    /// 胡桃用户选项
    /// </summary>
    public HutaoUserOptions UserOptions { get => hutaoUserOptions; }

    /// <summary>
    /// 选中的背景类型
    /// </summary>
    public NameValue<BackdropType>? SelectedBackdropType
    {
        get => selectedBackdropType ??= Options.BackdropTypes.Single(t => t.Value == Options.BackdropType);
        set
        {
            if (SetProperty(ref selectedBackdropType, value) && value is not null)
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
            if (SetProperty(ref selectedCulture, value) && value is not null)
            {
                Options.CurrentCulture = CultureInfo.GetCultureInfo(value.Value);
                AppInstance.Restart(string.Empty);
            }
        }
    }

    [Command("ResetStaticResourceCommand")]
    private static void ResetStaticResource()
    {
        StaticResource.FailAllContracts();
        LocalSetting.Set(SettingKeys.Major1Minor7Revision0GuideState, (uint)GuideState.StaticResourceBegin);
        AppInstance.Restart(string.Empty);
    }

    [Command("SetGamePathCommand")]
    private async Task SetGamePathAsync()
    {
        IGameLocator locator = gameLocatorFactory.Create(GameLocationSource.Manual);

        (bool isOk, string path) = await locator.LocateGamePathAsync().ConfigureAwait(false);
        if (isOk)
        {
            await taskContext.SwitchToMainThreadAsync();
            Options.GamePath = path;
        }
    }

    [Command("DeleteGameWebCacheCommand")]
    private void DeleteGameWebCache()
    {
        string gamePath = Options.GamePath;

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
                infoBarService.Warning(SH.ViewModelSettingClearWebCachePathInvalid.Format(cacheFolder));
            }
        }
    }

    [Command("ShowSignInWebViewDialogCommand")]
    private async Task ShowSignInWebViewDialogAsync()
    {
        // ContentDialog must be created by main thread.
        await taskContext.SwitchToMainThreadAsync();

        // TODO remove this.
        // SignInWebViewDialog dialog = serviceProvider.CreateInstance<SignInWebViewDialog>();
        // await dialog.ShowAsync();
    }

    [Command("UpdateCheckCommand")]
    private async Task CheckUpdateAsync()
    {
        if (hutaoUserOptions.IsMaintainer)
        {
            await navigationService
                .NavigateAsync<View.Page.TestPage>(INavigationAwaiter.Default)
                .ConfigureAwait(false);
        }
        else
        {
            await Launcher.LaunchUriAsync(new("ms-windows-store://pdp/?productid=9PH4NXJ2JN52"));
        }
    }

    [Command("SetDataFolderCommand")]
    private async Task SetDataFolderAsync()
    {
        (bool isOk, string folder) = await pickerFactory
            .GetFolderPicker()
            .TryPickSingleFolderAsync()
            .ConfigureAwait(false);

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

    [Command("NavigateToHutaoPassportCommand")]
    private void NavigateToHutaoPassport()
    {
        navigationService.Navigate<View.Page.HutaoPassportPage>(INavigationAwaiter.Default);
    }

    [Command("OpenCacheFolderCommand")]
    private Task OpenCacheFolderAsync()
    {
        return Launcher.LaunchFolderPathAsync(runtimeOptions.LocalCache).AsTask();
    }

    [Command("OpenDataFolderCommand")]
    private Task OpenDataFolderAsync()
    {
        return Launcher.LaunchFolderPathAsync(runtimeOptions.DataFolder).AsTask();
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
            infoBarService.Success("无感验证复合 Url 配置成功");
        }
    }
}