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
    private readonly string folder;

    public SettingFolderViewModel(ITaskContext taskContext, string folder)
    {
        this.taskContext = taskContext;
        this.folder = folder;

        _ = SetFolderSizeAsync();
    }

    public string Folder { get => folder; }

    public string? Size { get; set => SetProperty(ref field, value); }

    [SuppressMessage("", "SH003")]
    public async Task SetFolderSizeAsync()
    {
        await taskContext.SwitchToBackgroundAsync();
        long totalSize = 0;

        foreach (string file in Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories))
        {
            totalSize += new FileInfo(file).Length;
        }

        await taskContext.SwitchToMainThreadAsync();
        Size = SH.FormatViewModelSettingFolderSizeDescription(Converters.ToFileSizeString(totalSize));
    }

    [Command("OpenFolderCommand")]
    private async Task OpenDataFolderAsync()
    {
        await Launcher.LaunchFolderPathAsync(folder);
    }
}