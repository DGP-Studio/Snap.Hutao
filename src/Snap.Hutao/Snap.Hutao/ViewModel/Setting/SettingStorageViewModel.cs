// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.ViewModel.Guide;
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

    internal static async ValueTask<bool> InternalSetDataFolderAsync(IFileSystemPickerInteraction fileSystemPickerInteraction, IContentDialogFactory contentDialogFactory)
    {
        if (!fileSystemPickerInteraction.PickFolder().TryGetValue(out string? newFolder))
        {
            return false;
        }

        string oldFolder = HutaoRuntime.DataFolder;
        if (Path.GetFullPath(oldFolder).Equals(Path.GetFullPath(newFolder), StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        Directory.CreateDirectory(newFolder);
        if (Directory.EnumerateFileSystemEntries(newFolder).Any())
        {
            ContentDialogResult result = await contentDialogFactory.CreateForConfirmCancelAsync(
                SH.ViewModelSettingStorageSetDataFolderTitle,
                SH.ViewModelSettingStorageSetDataFolderDescription)
                .ConfigureAwait(false);
            if (result is not ContentDialogResult.Primary)
            {
                return false;
            }
        }

        HutaoException.ThrowIfNot(DirectoryOperation.Copy(oldFolder, newFolder), "Move DataFolder failed");
        LocalSetting.Set(SettingKeys.PreviousDataFolderToDelete, oldFolder);
        LocalSetting.Set(SettingKeys.DataFolderPath, newFolder);
        return true;
    }

    [Command("OpenBackgroundImageFolderCommand")]
    private static async Task OpenBackgroundImageFolderAsync()
    {
        await Launcher.LaunchFolderPathAsync(HutaoRuntime.GetDataFolderBackgroundFolder());
    }

    [Command("SetDataFolderCommand")]
    private async Task SetDataFolderAsync()
    {
        if (await InternalSetDataFolderAsync(fileSystemPickerInteraction, contentDialogFactory).ConfigureAwait(false))
        {
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
            await DataFolderView.SetFolderSizeAsync().ConfigureAwait(false);
        }

        infoBarService.Success(SH.ViewModelSettingActionComplete);
    }

    [Command("ResetStaticResourceCommand")]
    private async Task ResetStaticResource()
    {
        ContentDialog dialog = await contentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ViewModelSettingResetStaticResourceProgress)
            .ConfigureAwait(false);

        using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
        {
            await taskContext.SwitchToBackgroundAsync();
            StaticResource.FailAll();
            Directory.Delete(Path.Combine(HutaoRuntime.LocalCache, nameof(ImageCache)), true);
            UnsafeLocalSetting.Set(SettingKeys.GuideState, GuideState.StaticResourceBegin);
        }

        // TODO: prompt user that restart will be non-elevated
        AppInstance.Restart(string.Empty);
    }
}