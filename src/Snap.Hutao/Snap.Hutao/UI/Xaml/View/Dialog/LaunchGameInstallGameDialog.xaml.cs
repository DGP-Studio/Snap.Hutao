// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Package.Advanced;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;
using System.IO;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[ConstructorGenerated(InitializeComponent = true)]
[DependencyProperty("Chinese", typeof(bool))]
[DependencyProperty("English", typeof(bool))]
[DependencyProperty("Japanese", typeof(bool))]
[DependencyProperty("Korean", typeof(bool))]
[DependencyProperty("KnownSchemes", typeof(IList<LaunchScheme>))]
[DependencyProperty("SelectedScheme", typeof(LaunchScheme))]
[DependencyProperty("GameDirectory", typeof(string), default(string), nameof(OnGameDirectoryChanged))]
[DependencyProperty("IsParallelSupported", typeof(bool), true)]
internal sealed partial class LaunchGameInstallGameDialog : ContentDialog
{
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IInfoBarService infoBarService;

    public async ValueTask<ValueResult<bool, GameInstallOptions>> GetGameInstallOptionsAsync()
    {
        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        if (result is not ContentDialogResult.Primary)
        {
            return new(false, default!);
        }

        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();

        if (string.IsNullOrWhiteSpace(GameDirectory))
        {
            infoBarService.Error(SH.ViewDialogLaunchGameInstallGameDirectoryInvalid);
            return new(false, default!);
        }

        Directory.CreateDirectory(GameDirectory);
        if (!Directory.Exists(GameDirectory))
        {
            infoBarService.Error(SH.ViewDialogLaunchGameInstallGameDirectoryCreationFailed);
            return new(false, default!);
        }

        if (SelectedScheme is null)
        {
            infoBarService.Error(SH.ViewDialogLaunchGameInstallGameNoSchemeSelected);
            return new(false, default!);
        }

        if (!Chinese && !English && !Japanese && !Korean)
        {
            infoBarService.Error(SH.ViewDialogLaunchGameInstallGameNoAudioPackageSelected);
            return new(false, default!);
        }

        GameAudioSystem gameAudioSystem = new(Chinese, English, Japanese, Korean);
        string gamePath = Path.Combine(GameDirectory, SelectedScheme.IsOversea ? GameConstants.GenshinImpactFileName : GameConstants.YuanShenFileName);
        return new(true, new(GameFileSystem.CreateForPackageOperation(gamePath, gameAudioSystem), SelectedScheme));
    }

    private static void OnGameDirectoryChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ((LaunchGameInstallGameDialog)sender).IsParallelSupported = PhysicalDrive.GetIsSolidState((string)args.NewValue) ?? false;
    }

    [Command("PickGameDirectoryCommand")]
    private void PickGameDirectory()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Pick game directory", "LaunchGameInstallGameDialog.Command"));

        if (fileSystemPickerInteraction.PickFolder(SH.ViewDialogLaunchGameInstallGamePickDirectoryTitle) is (true, { } gameDirectory))
        {
            GameDirectory = gameDirectory;
        }
    }
}