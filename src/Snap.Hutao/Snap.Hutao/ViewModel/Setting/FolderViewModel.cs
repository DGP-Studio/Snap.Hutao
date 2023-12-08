// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using Windows.System;

namespace Snap.Hutao.ViewModel.Setting;

internal sealed partial class FolderViewModel : ObservableObject
{
    private readonly string folder;
    private string? size;

    public FolderViewModel(ITaskContext taskContext, string folder)
    {
        this.folder = folder;

        SetFolderSizeAsync().SafeForget();

        async ValueTask SetFolderSizeAsync()
        {
            long totalSize = 0;

            foreach (string file in Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories))
            {
                totalSize += new FileInfo(file).Length;
            }

            await taskContext.SwitchToMainThreadAsync();
            Size = SH.FormatViewModelSettingFolderSizeDescription(Converters.ToFileSizeString(totalSize));
        }
    }

    public string Folder { get => folder; }

    public string? Size { get => size; set => SetProperty(ref size, value); }

    [Command("OpenFolderCommand")]
    private async Task OpenDataFolderAsync()
    {
        await Launcher.LaunchFolderPathAsync(folder);
    }
}