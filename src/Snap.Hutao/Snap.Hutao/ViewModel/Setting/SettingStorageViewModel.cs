// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
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
using System.Runtime.InteropServices;
using Windows.System;

namespace Snap.Hutao.ViewModel.Setting;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class SettingStorageViewModel : Abstraction.ViewModel
{
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial SettingStorageViewModel(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial SettingFolderViewModel? CacheFolderView { get; set; }

    [ObservableProperty]
    public partial SettingFolderViewModel? DataFolderView { get; set; }

    [Command("OpenBackgroundImageFolderCommand")]
    private static async Task OpenBackgroundImageFolderAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Open background image folder", "SettingStorageViewModel.Command"));
        await Launcher.LaunchFolderPathAsync(HutaoRuntime.GetDataBackgroundDirectory());
    }

    [Command("SetDataFolderCommand")]
    private async Task SetDataFolderAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Set data folder path", "SettingStorageViewModel.Command"));

        SettingStorageSetDataFolderOperation operation = new()
        {
            FileSystemPickerInteraction = fileSystemPickerInteraction,
            ContentDialogFactory = contentDialogFactory,
            Messenger = messenger,
        };

        if (await operation.TryExecuteAsync().ConfigureAwait(false))
        {
            messenger.Send(InfoBarMessage.Success(SH.ViewModelSettingSetDataFolderSuccess));
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

        string cacheFolder = HutaoRuntime.GetDataServerCacheDirectory();
        if (Directory.Exists(cacheFolder))
        {
            Directory.Delete(cacheFolder, true);
        }

        if (DataFolderView is not null)
        {
            await DataFolderView.UpdateFolderSizeTimeoutAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        }

        messenger.Send(InfoBarMessage.Success(SH.ViewModelSettingActionComplete));
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
                Directory.Delete(Path.Combine(HutaoRuntime.LocalCacheDirectory, nameof(ImageCache)), true);
            }
            catch (DirectoryNotFoundException ex)
            {
                // Could not find a part of the path '.*?'.
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
        try
        {
            AppInstance.Restart(string.Empty);
        }
        catch (COMException ex)
        {
            if (HutaoNative.IsWin32(ex.HResult, WIN32_ERROR.ERROR_PACKAGE_UPDATING))
            {
                messenger.Send(InfoBarMessage.Error(SH.ViewModelSettingStorageRestartFailed));
            }
        }
    }
}