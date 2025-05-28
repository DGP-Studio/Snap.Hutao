// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Service.Notification;
using System.IO;
using Windows.Storage;

namespace Snap.Hutao.ViewModel.Setting;

internal sealed class SettingStorageSetDataFolderOperation
{
    public required IFileSystemPickerInteraction FileSystemPickerInteraction { private get; init; }

    public required IContentDialogFactory ContentDialogFactory { private get; init; }

    public required IInfoBarService InfoBarService { private get; init; }

    internal async ValueTask<bool> TryExecuteAsync()
    {
        if (!FileSystemPickerInteraction.PickFolder().TryGetValue(out string? newFolderPath))
        {
            return false;
        }

        string oldFolderPath = HutaoRuntime.DataFolder;
        if (Path.GetFullPath(oldFolderPath).Equals(Path.GetFullPath(newFolderPath), StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (Path.GetDirectoryName(newFolderPath) is null)
        {
            await ContentDialogFactory.CreateForConfirmAsync(
                    SH.ViewModelSettingStorageSetDataFolderTitle,
                    SH.ViewModelSettingStorageSetDataFolderDescription2)
                .ConfigureAwait(false);

            return false;
        }

        Directory.CreateDirectory(newFolderPath);
        if (Directory.EnumerateFileSystemEntries(newFolderPath).Any())
        {
            ContentDialogResult result = await ContentDialogFactory.CreateForConfirmCancelAsync(
                    SH.ViewModelSettingStorageSetDataFolderTitle,
                    SH.FormatViewModelSettingStorageSetDataFolderDescription3(newFolderPath))
                .ConfigureAwait(false);

            if (result is not ContentDialogResult.Primary)
            {
                return false;
            }
        }

        try
        {
            StorageFolder oldFolder = await StorageFolder.GetFolderFromPathAsync(oldFolderPath);
            await oldFolder.CopyAsync(newFolderPath).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            InfoBarService.Error(ex);
            return false;
        }

        LocalSetting.Set(SettingKeys.PreviousDataFolderToDelete, oldFolderPath);
        LocalSetting.Set(SettingKeys.DataFolderPath, newFolderPath);
        return true;
    }
}