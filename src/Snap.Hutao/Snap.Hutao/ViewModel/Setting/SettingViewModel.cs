// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Core.Shell;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Model;
using Snap.Hutao.Service;
using Snap.Hutao.Service.BackgroundImage;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Input.HotKey;
using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.UI.Xaml.Media.Backdrop;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.UI.Xaml.View.Page;
using Snap.Hutao.ViewModel.Guide;
using Snap.Hutao.Web.Hoyolab;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
    private readonly HutaoPassportViewModel hutaoPassportViewModel;
    private readonly BackgroundImageOptions backgroundImageOptions;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly INavigationService navigationService;
    private readonly IShellLinkInterop shellLinkInterop;
    private readonly HutaoUserOptions hutaoUserOptions;
    private readonly IInfoBarService infoBarService;
    private readonly CultureOptions cultureOptions;
    private readonly RuntimeOptions runtimeOptions;
    private readonly LaunchOptions launchOptions;
    private readonly HotKeyOptions hotKeyOptions;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;
    private readonly IMessenger messenger;

    private NameValue<BackdropType>? selectedBackdropType;
    private NameValue<ElementTheme>? selectedElementTheme;
    private NameValue<BackgroundImageType>? selectedBackgroundImageType;
    private NameValue<CultureInfo>? selectedCulture;
    private NameValue<Region>? selectedRegion;
    private FolderViewModel? cacheFolderView;
    private FolderViewModel? dataFolderView;

    public AppOptions AppOptions { get => appOptions; }

    public CultureOptions CultureOptions { get => cultureOptions; }

    public RuntimeOptions RuntimeOptions { get => runtimeOptions; }

    public HutaoUserOptions UserOptions { get => hutaoUserOptions; }

    public HomeCardOptions HomeCardOptions { get => homeCardOptions; }

    public HotKeyOptions HotKeyOptions { get => hotKeyOptions; }

    public LaunchOptions LaunchOptions { get => launchOptions; }

    public BackgroundImageOptions BackgroundImageOptions { get => backgroundImageOptions; }

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

    public NameValue<ElementTheme>? SelectedElementTheme
    {
        get => selectedElementTheme ??= AppOptions.LazyElementThemes.Value.Single(t => t.Value == AppOptions.ElementTheme);
        set
        {
            if (SetProperty(ref selectedElementTheme, value) && value is not null)
            {
                AppOptions.ElementTheme = value.Value;
            }
        }
    }

    public NameValue<BackgroundImageType>? SelectedBackgroundImageType
    {
        get => selectedBackgroundImageType ??= AppOptions.BackgroundImageTypes.Single(t => t.Value == AppOptions.BackgroundImageType);
        set
        {
            if (SetProperty(ref selectedBackgroundImageType, value) && value is not null)
            {
                AppOptions.BackgroundImageType = value.Value;
            }
        }
    }

    public NameValue<CultureInfo>? SelectedCulture
    {
        get => selectedCulture ??= CultureOptions.GetCurrentCultureForSelectionOrDefault();
        set
        {
            if (SetProperty(ref selectedCulture, value) && value is not null)
            {
                CultureOptions.CurrentCulture = value.Value;
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

    public bool IsAllocConsoleDebugModeEnabled
    {
        get => LocalSetting.Get(SettingKeys.IsAllocConsoleDebugModeEnabled, ConsoleWindowLifeTime.DebugModeEnabled);
        set
        {
            if (IsViewDisposed)
            {
                return;
            }

            ConfirmSetIsAllocConsoleDebugModeEnabledAsync(value).SafeForget();

            async ValueTask ConfirmSetIsAllocConsoleDebugModeEnabledAsync(bool value)
            {
                if (value)
                {
                    ReconfirmDialog dialog = await contentDialogFactory.CreateInstanceAsync<ReconfirmDialog>().ConfigureAwait(false);
                    if (await dialog.ConfirmAsync(SH.ViewSettingAllocConsoleHeader).ConfigureAwait(true))
                    {
                        LocalSetting.Set(SettingKeys.IsAllocConsoleDebugModeEnabled, true);
                        OnPropertyChanged(nameof(IsAllocConsoleDebugModeEnabled));
                        return;
                    }
                }

                LocalSetting.Set(SettingKeys.IsAllocConsoleDebugModeEnabled, false);
                OnPropertyChanged(nameof(IsAllocConsoleDebugModeEnabled));
            }
        }
    }

    public bool IsAdvancedLaunchOptionsEnabled
    {
        get => launchOptions.IsAdvancedLaunchOptionsEnabled;
        set
        {
            if (IsViewDisposed)
            {
                return;
            }

            ConfirmSetIsAdvancedLaunchOptionsEnabledAsync(value).SafeForget();

            async ValueTask ConfirmSetIsAdvancedLaunchOptionsEnabledAsync(bool value)
            {
                if (value)
                {
                    ReconfirmDialog dialog = await contentDialogFactory.CreateInstanceAsync<ReconfirmDialog>().ConfigureAwait(false);
                    if (await dialog.ConfirmAsync(SH.ViewPageSettingIsAdvancedLaunchOptionsEnabledHeader).ConfigureAwait(true))
                    {
                        launchOptions.IsAdvancedLaunchOptionsEnabled = true;
                        OnPropertyChanged(nameof(IsAdvancedLaunchOptionsEnabled));
                        return;
                    }
                }

                launchOptions.IsAdvancedLaunchOptionsEnabled = false;
                OnPropertyChanged(nameof(IsAdvancedLaunchOptionsEnabled));
            }
        }
    }

    protected override ValueTask<bool> InitializeOverrideAsync()
    {
        CacheFolderView = new(taskContext, runtimeOptions.LocalCache);
        DataFolderView = new(taskContext, runtimeOptions.DataFolder);

        return ValueTask.FromResult(true);
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

    [Command("ResetStaticResourceCommand")]
    private async Task ResetStaticResource()
    {
        ContentDialog dialog = await contentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ViewModelSettingResetStaticResourceProgress)
            .ConfigureAwait(false);

        await using (await dialog.BlockAsync(taskContext).ConfigureAwait(false))
        {
            await taskContext.SwitchToBackgroundAsync();
            StaticResource.FailAll();
            Directory.Delete(Path.Combine(runtimeOptions.LocalCache, nameof(ImageCache)), true);
            UnsafeLocalSetting.Set(SettingKeys.Major1Minor10Revision0GuideState, GuideState.StaticResourceBegin);
        }

        AppInstance.Restart(string.Empty);
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
            .NavigateAsync<TestPage>(INavigationAwaiter.Default)
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

    [Command("DeleteServerCacheFolderCommand")]
    private async Task DeleteServerCacheFolderAsync()
    {
        ContentDialogResult result = await contentDialogFactory.CreateForConfirmCancelAsync(
            SH.ViewModelSettingDeleteServerCacheFolderTitle,
            SH.ViewModelSettingDeleteServerCacheFolderContent)
            .ConfigureAwait(false);

        if (result is ContentDialogResult.Primary)
        {
            string cacheFolder = runtimeOptions.GetDataFolderServerCacheFolder();
            if (Directory.Exists(cacheFolder))
            {
                Directory.Delete(cacheFolder, true);
            }

            if (DataFolderView is not null)
            {
                await DataFolderView.SetFolderSizeAsync().ConfigureAwait(false);
            }

            infoBarService.Success(SH.ViewModelSettingActionComplete);
        }
    }

    [Command("OpenBackgroundImageFolderCommand")]
    private async Task OpenBackgroundImageFolderAsync()
    {
        await Launcher.LaunchFolderPathAsync(runtimeOptions.GetDataFolderBackgroundFolder());
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
                await @unsafe.UnsafeRemoveAllUsersAsync().ConfigureAwait(false);
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
            infoBarService.Success(SH.ViewModelSettingActionComplete);
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