// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.ViewModel.Guide;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using System.IO;
using Windows.System;

namespace Snap.Hutao.ViewModel.Setting;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class SettingStorageViewModel : Abstraction.ViewModel
{
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    public SettingFolderViewModel? CacheFolderView { get; set => SetProperty(ref field, value); }

    public SettingFolderViewModel? DataFolderView { get; set => SetProperty(ref field, value); }

    [Command("OpenBackgroundImageFolderCommand")]
    private static async Task OpenBackgroundImageFolderAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Open background image folder", "SettingStorageViewModel.Command"));
        await Launcher.LaunchFolderPathAsync(HutaoRuntime.GetDataFolderBackgroundFolder());
    }

    [Command("SetDataFolderCommand")]
    private async Task SetDataFolderAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Set data folder path", "SettingStorageViewModel.Command"));

        SettingStorageSetDataFolderOperation operation = new()
        {
            FileSystemPickerInteraction = fileSystemPickerInteraction,
            ContentDialogFactory = contentDialogFactory,
            InfoBarService = infoBarService,
        };

        if (await operation.TryExecuteAsync().ConfigureAwait(false))
        {
            infoBarService.Success(SH.ViewModelSettingSetDataFolderSuccess);
        }
    }

    [Command("DeleteServerCacheFolderCommand")]
    private async Task DeleteServerCacheFolderAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Delete server cache folder", "SettingStorageViewModel.Command"));

        ContentDialogResult result = await contentDialogFactory.CreateForConfirmCancelAsync(
            SH.ViewModelSettingDeleteServerCacheFolderTitle,
            SH.ViewModelSettingDeleteServerCacheFolderContent)
            .ConfigureAwait(false);

        if (result is not ContentDialogResult.Primary)
        {
            return;
        }

        string cacheFolder = HutaoRuntime.GetDataFolderServerCacheFolder();
        if (Directory.Exists(cacheFolder))
        {
            Directory.Delete(cacheFolder, true);
        }

        if (DataFolderView is not null)
        {
            await DataFolderView.SetFolderSizeTimeoutAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        }

        infoBarService.Success(SH.ViewModelSettingActionComplete);
    }

    [Command("ResetStaticResourceCommand")]
    private async Task ResetStaticResource()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Reset static resource", "SettingStorageViewModel.Command"));

        ContentDialog dialog = await contentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ViewModelSettingResetStaticResourceProgress)
            .ConfigureAwait(false);

        using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
        {
            await taskContext.SwitchToBackgroundAsync();
            StaticResource.FailAll();
            try
            {
                Directory.Delete(Path.Combine(HutaoRuntime.LocalCache, nameof(ImageCache)), true);
            }
            catch (DirectoryNotFoundException ex)
            {
                if (HutaoNative.IsWin32(ex.HResult, WIN32_ERROR.ERROR_PATH_NOT_FOUND))
                {
                    return;
                }

                SentrySdk.CaptureException(ex);
            }
            catch (IOException ex)
            {
                if (HutaoNative.IsWin32(ex.HResult, WIN32_ERROR.ERROR_SHARING_VIOLATION))
                {
                    return;
                }

                SentrySdk.CaptureException(ex);
            }

            UnsafeLocalSetting.Set(SettingKeys.GuideState, GuideState.StaticResourceBegin);
        }

        // TODO: prompt user that restart will be non-elevated
        AppInstance.Restart(string.Empty);
    }
}