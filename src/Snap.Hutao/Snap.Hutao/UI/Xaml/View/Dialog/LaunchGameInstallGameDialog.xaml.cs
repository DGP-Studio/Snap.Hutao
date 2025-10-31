// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Package.Advanced;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hoyolab.Downloader;
using Snap.Hutao.Web.Response;
using System.IO;
using System.Runtime.CompilerServices;
using WinRT;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty<bool>("Chinese", NotNull = true)]
[DependencyProperty<bool>("English", NotNull = true)]
[DependencyProperty<bool>("Japanese", NotNull = true)]
[DependencyProperty<bool>("Korean", NotNull = true)]
[DependencyProperty<IList<LaunchScheme>>("KnownSchemes", CreateDefaultValueCallbackName = nameof(CreateDefaultKnownSchemesValue))]
[DependencyProperty<LaunchScheme>("SelectedScheme", CreateDefaultValueCallbackName = nameof(CreateDefaultSelectedSchemeValue))]
[DependencyProperty<string>("GameDirectory", PropertyChangedCallbackName = nameof(OnGameDirectoryChanged))]
[DependencyProperty<bool>("IsParallelSupported", DefaultValue = true, NotNull = true)]
[DependencyProperty<bool>("IsBetaGameInstallEnabled", CreateDefaultValueCallbackName = nameof(CreateDefaultIsBetaGameInstallEnabledValue), NotNull = true)]
[DependencyProperty<bool>("IsBetaGameInstall", DefaultValue = false, PropertyChangedCallbackName = nameof(OnIsBetaGameInstallChanged), NotNull = true)]
[DependencyProperty<string>("BetaBuildBodyFilePath")]
internal sealed partial class LaunchGameInstallGameDialog : ContentDialog
{
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly JsonSerializerOptions jsonSerializerOptions;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IMessenger messenger;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial LaunchGameInstallGameDialog(IServiceProvider serviceProvider);

    public async ValueTask<ValueResult<bool, GameInstallOptions?>> GetGameInstallOptionsAsync()
    {
        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        if (result is not ContentDialogResult.Primary)
        {
            return new(false, default);
        }

        await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();

        if (string.IsNullOrWhiteSpace(GameDirectory))
        {
            messenger.Send(InfoBarMessage.Error(SH.ViewDialogLaunchGameInstallGameDirectoryInvalid));
            return new(false, default);
        }

        Directory.CreateDirectory(GameDirectory);
        if (!Directory.Exists(GameDirectory))
        {
            messenger.Send(InfoBarMessage.Error(SH.ViewDialogLaunchGameInstallGameDirectoryCreationFailed));
            return new(false, default);
        }

        if (SelectedScheme is null)
        {
            messenger.Send(InfoBarMessage.Error(SH.ViewDialogLaunchGameInstallGameNoSchemeSelected));
            return new(false, default);
        }

        if (!(Chinese || English || Japanese || Korean))
        {
            messenger.Send(InfoBarMessage.Error(SH.ViewDialogLaunchGameInstallGameNoAudioPackageSelected));
            return new(false, default);
        }

        GameAudioInstallation gameAudioInstallation = new(Chinese, English, Japanese, Korean);
        string gamePath = Path.Combine(GameDirectory, SelectedScheme.IsOversea ? GameConstants.GenshinImpactFileName : GameConstants.YuanShenFileName);

        if (IsBetaGameInstall)
        {
            if (!File.Exists(BetaBuildBodyFilePath))
            {
                messenger.Send(InfoBarMessage.Error("The build body file does not exist."));
                return new(false, default);
            }

            SophonBuild? build;
            using (FileStream stream = File.OpenRead(BetaBuildBodyFilePath))
            {
                build = (await JsonSerializer.DeserializeAsync<Response<SophonBuild>>(stream, jsonSerializerOptions).ConfigureAwait(true))?.Data;
            }

            if (build is null)
            {
                messenger.Send(InfoBarMessage.Error("Failed to parse the build body file."));
                return new(false, default);
            }

            return new(true, new(GameFileSystem.CreateForPackageOperation(gamePath, gameAudioInstallation), SelectedScheme, build));
        }

        return new(true, new(GameFileSystem.CreateForPackageOperation(gamePath, gameAudioInstallation), SelectedScheme));
    }

    private static object CreateDefaultIsBetaGameInstallEnabledValue()
    {
        return LocalSetting.Get(SettingKeys.EnableBetaGameInstall, false);
    }

    private static object CreateDefaultKnownSchemesValue()
    {
        return KnownLaunchSchemes.Values;
    }

    private static object CreateDefaultSelectedSchemeValue()
    {
        return KnownLaunchSchemes.Values.First(s => s.IsNotCompatOnly);
    }

    private static void OnGameDirectoryChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        LaunchGameInstallGameDialog dialog = sender.As<LaunchGameInstallGameDialog>();
        bool? isSSD = PhysicalDrive.GetIsSolidState(Unsafe.As<string>(args.NewValue));
        if (isSSD is null)
        {
            dialog.messenger.Send(InfoBarMessage.Error(SH.ViewDialogLaunchGameInstallGameDirectoryIsSSDCheckFailed));
        }

        dialog.IsParallelSupported = PhysicalDrive.GetIsSolidState(Unsafe.As<string>(args.NewValue)) ?? false;
    }

    private static void OnIsBetaGameInstallChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        LaunchGameInstallGameDialog dialog = sender.As<LaunchGameInstallGameDialog>();
        dialog.KnownSchemes = Unsafe.Unbox<bool>(args.NewValue) ? KnownLaunchSchemes.BetaValues : KnownLaunchSchemes.Values;
        dialog.SelectedScheme = dialog.KnownSchemes.First(s => s.IsNotCompatOnly);
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

    [Command("PickBetaBuildBodyFilePathCommand")]
    private void PickGetBuildBodyFilePath()
    {
        if (fileSystemPickerInteraction.PickFile("GetBuildWithStokenLogin Body File", "JSON", "*.json") is (true, var getBuildBodyFilePath))
        {
            BetaBuildBodyFilePath = getBuildBodyFilePath;
        }
    }
}