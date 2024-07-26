// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.Control;
using System.IO;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty("Chinese", typeof(bool))]
[DependencyProperty("English", typeof(bool))]
[DependencyProperty("Japanese", typeof(bool))]
[DependencyProperty("Korean", typeof(bool))]
[DependencyProperty("KnownSchemes", typeof(IEnumerable<LaunchScheme>))]
[DependencyProperty("SelectedScheme", typeof(LaunchScheme))]
[DependencyProperty("GameDirectory", typeof(string))]
internal sealed partial class LaunchGameInstallGameDialog : ContentDialog
{
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    public LaunchGameInstallGameDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        fileSystemPickerInteraction = serviceProvider.GetRequiredService<IFileSystemPickerInteraction>();
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
    }

    public async ValueTask<ValueResult<bool, GameFileSystem>> GetGameFileSystemAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        if (result is not ContentDialogResult.Primary)
        {
            return new(false, default!);
        }

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
        return new(true, new(gamePath, gameAudioSystem));
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
