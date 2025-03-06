// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using Windows.System;

namespace Snap.Hutao.ViewModel.Setting;

internal sealed partial class SettingFolderViewModel : ObservableObject
{
    private readonly ITaskContext taskContext;

    public SettingFolderViewModel(ITaskContext taskContext, string folder)
    {
        this.taskContext = taskContext;
        Folder = folder;

        _ = SetFolderSizeTimeoutAsync(TimeSpan.FromSeconds(5));
    }

    public string Folder { get; }

    [ObservableProperty]
    public partial string? Size { get; set; }

    [SuppressMessage("", "SH003")]
    public async Task SetFolderSizeTimeoutAsync(TimeSpan timeout)
    {
        // We don't want this function to run indefinitely in principle,
        // users can have a lot of files in the folder if they manually put them in
        using (CancellationTokenSource source = new(timeout))
        {
            await SetFolderSizeAsync(source.Token).ConfigureAwait(false);
        }
    }

    private async ValueTask SetFolderSizeAsync(CancellationToken token)
    {
        await taskContext.SwitchToBackgroundAsync();
        long totalSize = 0;

        if (!Directory.Exists(Folder))
        {
            return;
        }

        foreach (string file in Directory.EnumerateFiles(Folder, "*.*", SearchOption.AllDirectories))
        {
            token.ThrowIfCancellationRequested();

            try
            {
                totalSize += new FileInfo(file).Length;
            }
            catch (UnauthorizedAccessException)
            {
                // Mostly 'System Volume Information' folder,
                // Users prefer to store their data in root directory
                // For all situations, we can't do anything about it
            }
        }

        await taskContext.SwitchToMainThreadAsync();
        Size = SH.FormatViewModelSettingFolderSizeDescription(Converters.ToFileSizeString(totalSize));
    }

    [Command("OpenFolderCommand")]
    private async Task OpenDataFolderAsync()
    {
        await Launcher.LaunchFolderPathAsync(Folder);
    }
}