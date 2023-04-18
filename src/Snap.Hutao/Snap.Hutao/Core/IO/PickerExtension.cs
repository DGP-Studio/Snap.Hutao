// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Storage;
using Windows.Storage.Pickers;

namespace Snap.Hutao.Core.IO;

/// <summary>
/// 选择器拓展
/// </summary>
internal static class PickerExtension
{
    /// <inheritdoc cref="FileOpenPicker.PickSingleFileAsync"/>
    public static async Task<ValueResult<bool, ValueFile>> TryPickSingleFileAsync(this FileOpenPicker picker)
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

        if (file != null)
        {
            return new(true, file.Path);
        }
        else
        {
            InfoBarWaringPickerException(exception);
            return new(false, null!);
        }
    }

    /// <inheritdoc cref="FileSavePicker.PickSaveFileAsync"/>
    public static async Task<ValueResult<bool, ValueFile>> TryPickSaveFileAsync(this FileSavePicker picker)
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

        if (file != null)
        {
            return new(true, file.Path);
        }
        else
        {
            InfoBarWaringPickerException(exception);
            return new(false, null!);
        }
    }

    /// <inheritdoc cref="FolderPicker.PickSingleFolderAsync"/>
    public static async Task<ValueResult<bool, string>> TryPickSingleFolderAsync(this FolderPicker picker)
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

        if (folder != null)
        {
            return new(true, folder.Path);
        }
        else
        {
            InfoBarWaringPickerException(exception);
            return new(false, null!);
        }
    }

    private static void InfoBarWaringPickerException(Exception? exception)
    {
        if (exception != null)
        {
            Ioc.Default
                .GetRequiredService<Service.Abstraction.IInfoBarService>()
                .Warning(
                    SH.CoreIOPickerExtensionPickerExceptionInfoBarTitle,
                    string.Format(SH.CoreIOPickerExtensionPickerExceptionInfoBarMessage, exception.Message));
        }
    }
}