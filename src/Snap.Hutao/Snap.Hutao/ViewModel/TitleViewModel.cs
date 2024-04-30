// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Windowing.HotKey;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.Update;
using Snap.Hutao.View.Dialog;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Snap.Hutao.ViewModel;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class TitleViewModel : Abstraction.ViewModel
{
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
                .AppendIf(runtimeOptions.IsElevated, "Elevated")
#if DEBUG
                .Append("Dev")
#endif
                .Append("NameAndVersion")
                .ToString();

            string? format = SH.GetString(CultureInfo.CurrentCulture, name);
            ArgumentException.ThrowIfNullOrEmpty(format);
            return string.Format(CultureInfo.CurrentCulture, format, runtimeOptions.Version);
        }
    }

    public UpdateStatus? UpdateStatus { get => updateStatus; set => SetProperty(ref updateStatus, value); }

    protected override async ValueTask<bool> InitializeUIAsync()
    {
        await DoCheckUpdateAsync().ConfigureAwait(false);
        return true;
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

            if (await dialog.ShowAsync() is ContentDialogResult.Primary)
            {
                // This method will set CheckUpdateResult.Kind to NeedInstall if download success
                if (!await DownloadPackageAsync(progress, checkUpdateResult).ConfigureAwait(false))
                {
                    infoBarService.Warning(SH.ViewTitileUpdatePackageDownloadFailedMessage);
                    return;
                }
            }
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
                LaunchUpdaterResult launchUpdaterResult = await updateService.LaunchUpdaterAsync().ConfigureAwait(false);
                if (launchUpdaterResult.IsSuccess)
                {
                    ContentDialog contentDialog = await contentDialogFactory
                        .CreateForIndeterminateProgressAsync(SH.ViewTitleUpdatePackageInstallingContent)
                        .ConfigureAwait(false);
                    using (await contentDialog.BlockAsync(taskContext).ConfigureAwait(false))
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

    private async ValueTask<bool> DownloadPackageAsync(IProgress<UpdateStatus> progress, CheckUpdateResult checkUpdateResult)
    {
        bool downloadSuccess = true;
        try
        {
            if (await updateService.DownloadUpdateAsync(checkUpdateResult, progress).ConfigureAwait(false))
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
}