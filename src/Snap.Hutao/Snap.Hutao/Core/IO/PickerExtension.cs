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
    public static async Task<ValueResult<bool, FilePath>> TryPickSingleFileAsync(this FileOpenPicker picker)
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
            if (exception != null)
            {
                Ioc.Default
                    .GetRequiredService<Service.Abstraction.IInfoBarService>()
                    .Warning($"无法打开文件选择器 {exception.Message}");
            }

            return new(false, null!);
        }
    }

    /// <inheritdoc cref="FileSavePicker.PickSaveFileAsync"/>
    public static async Task<ValueResult<bool, FilePath>> TryPickSaveFileAsync(this FileSavePicker picker)
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
            if (exception != null)
            {
                Ioc.Default
                    .GetRequiredService<Service.Abstraction.IInfoBarService>()
                    .Warning($"无法打开文件选择器 {exception.Message}");
            }

            return new(false, null!);
        }
    }
}