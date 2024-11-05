// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Caching;
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

    private SettingFolderViewModel? cacheFolderView;
    private SettingFolderViewModel? dataFolderView;

    public SettingFolderViewModel? CacheFolderView { get => cacheFolderView; set => SetProperty(ref cacheFolderView, value); }

    public SettingFolderViewModel? DataFolderView { get => dataFolderView; set => SetProperty(ref dataFolderView, value); }

    [Command("OpenBackgroundImageFolderCommand")]
    private async Task OpenBackgroundImageFolderAsync()
    {
        await Launcher.LaunchFolderPathAsync(HutaoRuntime.GetDataFolderBackgroundFolder());
    }

    [Command("SetDataFolderCommand")]
    private void SetDataFolder()
    {
        if (fileSystemPickerInteraction.PickFolder().TryGetValue(out string? folder))
        {
            LocalSetting.Set(SettingKeys.DataFolderPath, folder);
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
            UnsafeLocalSetting.Set(SettingKeys.Major1Minor10Revision0GuideState, GuideState.StaticResourceBegin);
        }

        // TODO: prompt user that restart will be non-elevated
        AppInstance.Restart(string.Empty);
    }
}