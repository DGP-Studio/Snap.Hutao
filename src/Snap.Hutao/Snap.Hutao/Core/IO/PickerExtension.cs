// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Snap.Hutao.Core.IO;

/// <summary>
/// 选择器拓展
/// </summary>
internal static class PickerExtension
{
    /// <inheritdoc cref="FileOpenPicker.PickSingleFileAsync"/>
    public static async ValueTask<ValueResult<bool, ValueFile>> TryPickSingleFileAsync(this FileOpenPicker picker)
    {
        StorageFile? file;
        Exception? exception = null;
        try
        {
            file = await picker.PickSingleFileAsync().AsTask().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            exception = ex;
            file = null;
        }

        if (file is not null)
        {
            return new(true, file.Path);
        }
        else
        {
            InfoBarWaringPickerException(exception);
            return new(false, default!);
        }
    }

    /// <inheritdoc cref="FileSavePicker.PickSaveFileAsync"/>
    public static async ValueTask<ValueResult<bool, ValueFile>> TryPickSaveFileAsync(this FileSavePicker picker)
    {
        StorageFile? file;
        Exception? exception = null;
        try
        {
            file = await picker.PickSaveFileAsync().AsTask().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            exception = ex;
            file = null;
        }

        if (file is not null)
        {
            return new(true, file.Path);
        }
        else
        {
            InfoBarWaringPickerException(exception);
            return new(false, default!);
        }
    }

    /// <inheritdoc cref="FolderPicker.PickSingleFolderAsync"/>
    public static async ValueTask<ValueResult<bool, string>> TryPickSingleFolderAsync(this FolderPicker picker)
    {
        StorageFolder? folder;
        Exception? exception = null;
        try
        {
            folder = await picker.PickSingleFolderAsync().AsTask().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            exception = ex;
            folder = null;
        }

        if (folder is not null)
        {
            return new(true, folder.Path);
        }
        else
        {
            InfoBarWaringPickerException(exception);
            return new(false, default!);
        }
    }

    private static void InfoBarWaringPickerException(Exception? exception)
    {
        if (exception is not null)
        {
            Ioc.Default
                .GetRequiredService<IInfoBarService>()
                .Warning(
                    SH.CoreIOPickerExtensionPickerExceptionInfoBarTitle,
                    string.Format(SH.CoreIOPickerExtensionPickerExceptionInfoBarMessage, exception.Message));
        }
    }
}