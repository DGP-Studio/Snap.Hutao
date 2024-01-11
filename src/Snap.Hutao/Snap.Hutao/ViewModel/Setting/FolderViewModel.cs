// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using Windows.System;

namespace Snap.Hutao.ViewModel.Setting;

internal sealed partial class FolderViewModel : ObservableObject
{
    private readonly ITaskContext taskContext;
    private readonly string folder;
    private string? size;

    public FolderViewModel(ITaskContext taskContext, string folder)
    {
        this.taskContext = taskContext;
        this.folder = folder;

        SetFolderSizeAsync().SafeForget();
    }

    public string Folder { get => folder; }

    public string? Size { get => size; set => SetProperty(ref size, value); }

    public async ValueTask SetFolderSizeAsync()
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