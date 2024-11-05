// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.Update;
using Snap.Hutao.UI.Input.HotKey;
using Snap.Hutao.UI.Xaml.Behavior.Action;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.UI.Xaml.View.Window.WebView2;
using Snap.Hutao.Web.Hutao;
using System.Globalization;
using System.IO;
using System.Text;

namespace Snap.Hutao.ViewModel;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class TitleViewModel : Abstraction.ViewModel
{
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IProgressFactory progressFactory;
    private readonly IInfoBarService infoBarService;
    private readonly RuntimeOptions runtimeOptions;
    private readonly HotKeyOptions hotKeyOptions;
    private readonly IUpdateService updateService;
    private readonly ITaskContext taskContext;

    private UpdateStatus? updateStatus;

    public RuntimeOptions RuntimeOptions { get => runtimeOptions; }

    public HotKeyOptions HotKeyOptions { get => hotKeyOptions; }

    public string Title
    {
        [SuppressMessage("", "IDE0027")]
        get
        {
            string name = new StringBuilder()
                .Append("App")
                .AppendIf(HutaoRuntime.IsProcessElevated, "Elevated")
#if DEBUG
                .Append("Dev")
#endif
                .Append("NameAndVersion")
                .ToString();

            string? format = SH.GetString(CultureInfo.CurrentCulture, name);
            ArgumentException.ThrowIfNullOrEmpty(format);
            return string.Format(CultureInfo.CurrentCulture, format, HutaoRuntime.Version);
        }
    }

    public UpdateStatus? UpdateStatus { get => updateStatus; set => SetProperty(ref updateStatus, value); }

    protected override async ValueTask<bool> LoadOverrideAsync()
    {
        ShowUpdateLogWindowAfterUpdate();
        NotifyIfDateFolderHasReparsePoint();
        await DoCheckUpdateAsync().ConfigureAwait(false);
        return true;
    }

    private void ShowUpdateLogWindowAfterUpdate()
    {
        if (LocalSetting.Get(SettingKeys.AlwaysIsFirstRunAfterUpdate, false) || XamlApplicationLifetime.IsFirstRunAfterUpdate)
        {
            XamlApplicationLifetime.IsFirstRunAfterUpdate = false;
            ShowWebView2WindowAction.Show<UpdateLogContentProvider>(currentXamlWindowReference.GetXamlRoot());
        }
    }

    private async ValueTask DoCheckUpdateAsync()
    {
        IProgress<UpdateStatus> progress = progressFactory.CreateForMainThread<UpdateStatus>(status => UpdateStatus = status);
        CheckUpdateResult checkUpdateResult = await updateService.CheckUpdateAsync(progress).ConfigureAwait(false);

        if (checkUpdateResult.Kind is CheckUpdateResultKind.NeedDownload)
        {
            UpdatePackageDownloadConfirmDialog dialog = await contentDialogFactory
                .CreateInstanceAsync<UpdatePackageDownloadConfirmDialog>()
                .ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();

            dialog.Title = SH.FormatViewTitileUpdatePackageDownloadTitle(UpdateStatus?.Version);
            dialog.Mirrors = checkUpdateResult.PackageInformation?.Mirrors;
            dialog.SelectedItem = dialog.Mirrors?.FirstOrDefault();

            (bool isOk, HutaoPackageMirror? mirror) = await dialog.GetSelectedMirrorAsync().ConfigureAwait(false);

            if (isOk && mirror is not null)
            {
                ArgumentNullException.ThrowIfNull(checkUpdateResult.PackageInformation);
                HutaoSelectedMirrorInformation mirrorInformation = new()
                {
                    Mirror = mirror,
                    Validation = checkUpdateResult.PackageInformation.Validation,
                    Version = checkUpdateResult.PackageInformation.Version,
                };

                // This method will set CheckUpdateResult.Kind to NeedInstall if download success
                if (!await DownloadPackageAsync(progress, mirrorInformation, checkUpdateResult).ConfigureAwait(false))
                {
                    infoBarService.Warning(SH.ViewTitileUpdatePackageDownloadFailedMessage);
                    return;
                }
            }
        }

        if (currentXamlWindowReference.Window is null)
        {
            return;
        }

        if (checkUpdateResult.Kind is CheckUpdateResultKind.NeedInstall)
        {
            ContentDialogResult installUpdateUserConsentResult = await contentDialogFactory
                .CreateForConfirmCancelAsync(
                    SH.FormatViewTitileUpdatePackageReadyTitle(UpdateStatus?.Version),
                    SH.ViewTitileUpdatePackageReadyContent,
                    ContentDialogButton.Primary)
                .ConfigureAwait(false);

            if (installUpdateUserConsentResult is ContentDialogResult.Primary)
            {
                LaunchUpdaterResult launchUpdaterResult = updateService.LaunchUpdater();
                if (launchUpdaterResult.IsSuccess)
                {
                    ContentDialog contentDialog = await contentDialogFactory
                        .CreateForIndeterminateProgressAsync(SH.ViewTitleUpdatePackageInstallingContent)
                        .ConfigureAwait(false);
                    using (await contentDialogFactory.BlockAsync(contentDialog).ConfigureAwait(false))
                    {
                        if (launchUpdaterResult.Process is { } updater)
                        {
                            await updater.WaitForExitAsync().ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        await taskContext.SwitchToMainThreadAsync();
        UpdateStatus = null;
    }

    private async ValueTask<bool> DownloadPackageAsync(IProgress<UpdateStatus> progress, HutaoSelectedMirrorInformation mirrorInformation, CheckUpdateResult checkUpdateResult)
    {
        bool downloadSuccess = true;
        try
        {
            if (await updateService.DownloadUpdateAsync(mirrorInformation, progress).ConfigureAwait(false))
            {
                checkUpdateResult.Kind = CheckUpdateResultKind.NeedInstall;
            }
            else
            {
                downloadSuccess = false;
            }
        }
        catch
        {
            downloadSuccess = false;
        }

        return downloadSuccess;
    }

    private void NotifyIfDateFolderHasReparsePoint()
    {
        if (new DirectoryInfo(HutaoRuntime.DataFolder).Attributes.HasFlag(FileAttributes.ReparsePoint))
        {
            infoBarService.Warning(SH.FormatViewModelTitleDataFolderHasReparsepoint(HutaoRuntime.DataFolder));
        }
    }
}