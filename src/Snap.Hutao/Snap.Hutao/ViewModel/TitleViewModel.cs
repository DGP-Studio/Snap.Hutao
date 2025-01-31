// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.IO.Http.Proxy;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.Update;
using Snap.Hutao.UI.Input.HotKey;
using Snap.Hutao.UI.Xaml.Behavior.Action;
using Snap.Hutao.UI.Xaml.Control.Theme;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.UI.Xaml.View.Window.WebView2;
using Snap.Hutao.Web.Hutao;
using System.IO;

namespace Snap.Hutao.ViewModel;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
[SuppressMessage("", "SA1201")]
internal sealed partial class TitleViewModel : Abstraction.ViewModel
{
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly HttpProxyUsingSystemProxy httpProxyUsingSystemProxy;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IMetadataService metadataService;
    private readonly IProgressFactory progressFactory;
    private readonly ILogger<TitleViewModel> logger;
    private readonly IInfoBarService infoBarService;
    private readonly IUpdateService updateService;
    private readonly ITaskContext taskContext;
    private readonly App app;

    public static string Title
    {
        get
        {
            string? title = HutaoRuntime.GetDisplayName();
            ArgumentException.ThrowIfNullOrEmpty(title);
            return title;
        }
    }

    public partial RuntimeOptions RuntimeOptions { get; }

    public partial HotKeyOptions HotKeyOptions { get; }

    public UpdateStatus? UpdateStatus { get; set => SetProperty(ref field, value); }

    public bool IsMetadataInitialized { get; set => SetProperty(ref field, value); }

    protected override async ValueTask<bool> LoadOverrideAsync()
    {
        ShowUpdateLogWindowAfterUpdate();
        NotifyIfDataFolderHasReparsePoint();
        WaitMetadataInitializationAsync().SafeForget(logger);
        await CheckUpdateAsync().ConfigureAwait(false);
        await CheckProxyAndLoopbackAsync().ConfigureAwait(false);
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

    private async ValueTask CheckUpdateAsync()
    {
        IProgress<UpdateStatus> progress = progressFactory.CreateForMainThread<UpdateStatus>(status => UpdateStatus = status);
        CheckUpdateResult checkUpdateResult = await updateService.CheckUpdateAsync(progress).ConfigureAwait(false);

        if (currentXamlWindowReference.Window is null)
        {
            return;
        }

        if (checkUpdateResult.Kind is CheckUpdateResultKind.NeedDownload)
        {
            UpdatePackageDownloadConfirmDialog dialog = await contentDialogFactory
                .CreateInstanceAsync<UpdatePackageDownloadConfirmDialog>()
                .ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();

            dialog.Title = SH.FormatViewTitileUpdatePackageDownloadTitle(UpdateStatus?.Version);
            dialog.Mirrors = checkUpdateResult.PackageInformation?.Mirrors;
            dialog.SelectedItem = dialog.Mirrors?.FirstOrDefault();

            if (await dialog.GetSelectedMirrorAsync().ConfigureAwait(false) is (true, { } mirror))
            {
                ArgumentNullException.ThrowIfNull(checkUpdateResult.PackageInformation);
                HutaoSelectedMirrorInformation mirrorInformation = new()
                {
                    Mirror = mirror,
                    Validation = checkUpdateResult.PackageInformation.Validation,
                    Version = checkUpdateResult.PackageInformation.Version,
                };

                // This method will
                // 1. set CheckUpdateResult.Kind to NeedInstall if download success
                // 2. set CheckUpdateResult.Kind to SkipInstall if selected mirror is browser
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

    private async ValueTask<bool> DownloadPackageAsync(IProgress<UpdateStatus> progress, HutaoSelectedMirrorInformation selectedMirrorInformation, CheckUpdateResult checkUpdateResult)
    {
        bool downloadSuccess = true;
        try
        {
            if (await updateService.DownloadUpdateAsync(selectedMirrorInformation, progress).ConfigureAwait(false))
            {
                checkUpdateResult.Kind = CheckUpdateResultKind.NeedInstall;
            }
            else
            {
                // DownloadUpdateAsync will return 'false' if mirror type is browser
                if (selectedMirrorInformation.Mirror.MirrorType is HutaoPackageMirrorType.Browser)
                {
                    // Since we haven't actually downloaded the package, the return value
                    // should remain true to prevent the warning message from showing up.
                    checkUpdateResult.Kind = CheckUpdateResultKind.SkipInstall;
                }
                else
                {
                    downloadSuccess = false;
                }
            }
        }
        catch
        {
            downloadSuccess = false;
        }

        return downloadSuccess;
    }

    private void NotifyIfDataFolderHasReparsePoint()
    {
        if (new DirectoryInfo(HutaoRuntime.DataFolder).Attributes.HasFlag(FileAttributes.ReparsePoint))
        {
            infoBarService.Warning(SH.FormatViewModelTitleDataFolderHasReparsepoint(HutaoRuntime.DataFolder));
        }
    }

    private async ValueTask CheckProxyAndLoopbackAsync()
    {
        if (!httpProxyUsingSystemProxy.IsUsingProxyAndNotWorking)
        {
            return;
        }

        ContentDialogResult result = await contentDialogFactory
            .CreateForConfirmCancelAsync(SH.ViewDialogFeedbackEnableLoopbackTitle, SH.ViewDialogFeedbackEnableLoopbackDescription)
            .ConfigureAwait(false);

        if (result is ContentDialogResult.Primary)
        {
            await taskContext.SwitchToMainThreadAsync();
            httpProxyUsingSystemProxy.EnableLoopback();
        }
    }

    private async ValueTask WaitMetadataInitializationAsync()
    {
        try
        {
            await metadataService.InitializeAsync().ConfigureAwait(false);
        }
        finally
        {
            await taskContext.SwitchToMainThreadAsync();
            IsMetadataInitialized = true;
        }
    }
}

internal sealed partial class TitleViewModel
{
    public static bool IsDebug
    {
        get =>
#if DEBUG
            true;
#else
            false;
#endif
    }

#if DEBUG
    [Command("InvertAppThemeCommand")]
    private void InvertAppTheme()
    {
        WinUI.FrameworkTheming.FrameworkTheming.SetTheme(ThemeHelper.ApplicationToFrameworkInvert(app.RequestedTheme));
    }
#endif
}