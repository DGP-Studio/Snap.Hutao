// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.IO;
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
[DependencyProperty("KnownSchemes", typeof(IEnumerable<LaunchScheme>))]
[DependencyProperty("SelectedScheme", typeof(LaunchScheme))]
[DependencyProperty("GameDirectory", typeof(string), default(string), nameof(OnGameDirectoryChanged))]
[DependencyProperty("IsParallelSupported", typeof(bool), true)]
internal sealed partial class LaunchGameInstallGameDialog : ContentDialog
{
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IInfoBarService infoBarService;

    public async ValueTask<ValueResult<bool, GameInstallOptions>> GetGameFileSystemAsync()
    {
        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        if (result is not ContentDialogResult.Primary)
        {
            return new(false, default!);
        }

        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();

        if (string.IsNullOrWhiteSpace(GameDirectory))
        {
            infoBarService.Error("安装路径未选择");
            return new(false, default!);
        }

        if (SelectedScheme is null)
        {
            infoBarService.Error("游戏区服未选择");
            return new(false, default!);
        }

        if (!Chinese && !English && !Japanese && !Korean)
        {
            infoBarService.Error("语音包未选择");
            return new(false, default!);
        }

        GameAudioSystem gameAudioSystem = new(Chinese, English, Japanese, Korean);
        string gamePath = Path.Combine(GameDirectory, SelectedScheme.IsOversea ? GameConstants.GenshinImpactFileName : GameConstants.YuanShenFileName);
        return new(true, new(new(gamePath, gameAudioSystem), SelectedScheme));
    }

    private static void OnGameDirectoryChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        // TODO: refine infobar title
        LaunchGameInstallGameDialog dialog = (LaunchGameInstallGameDialog)sender;
        dialog.IsParallelSupported = PhysicalDriver.DangerousGetIsSolidState((string)args.NewValue);
    }

    [Command("PickGameDirectoryCommand")]
    private void PickGameDirectory()
    {
        (bool isPickerOk, string gameDirectory) = fileSystemPickerInteraction.PickFolder("选择安装路径");

        if (isPickerOk)
        {
            GameDirectory = gameDirectory;
        }
    }
}
