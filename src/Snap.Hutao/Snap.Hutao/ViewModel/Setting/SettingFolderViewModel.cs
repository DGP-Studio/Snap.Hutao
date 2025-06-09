// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.Logging;
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

        SetFolderSizeTimeoutAsync(TimeSpan.FromSeconds(5)).SafeForget();
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

        totalSize = DirectoryOperation.GetSize(Folder, token);
        await taskContext.SwitchToMainThreadAsync();
        Size = SH.FormatViewModelSettingFolderSizeDescription(Converters.ToFileSizeString(totalSize));
    }

    [Command("OpenFolderCommand")]
    private async Task OpenFolderAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Open folder", "SettingFolderViewModel.Command"));
        await Launcher.LaunchFolderPathAsync(Folder);
    }
}