// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.Update;
using Snap.Hutao.UI.Input.HotKey;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.UI.Xaml.Behavior.Action;
using Snap.Hutao.UI.Xaml.Control.Theme;
using Snap.Hutao.UI.Xaml.View.Window.WebView2;
using System.IO;

namespace Snap.Hutao.ViewModel;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
[SuppressMessage("", "SA1201")]
internal sealed partial class TitleViewModel : Abstraction.ViewModel
{
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
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

    public bool IsMetadataInitialized { get; set => SetProperty(ref field, value); }

    protected override async ValueTask<bool> LoadOverrideAsync()
    {
        ShowUpdateLogWindowAfterUpdate();
        NotifyIfDataFolderHasReparsePoint();
        WaitMetadataInitializationAsync().SafeForget(logger);
        await CheckUpdateAsync().ConfigureAwait(false);
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
        CheckUpdateResult checkUpdateResult = await updateService.CheckUpdateAsync().ConfigureAwait(false);

        if (currentXamlWindowReference.Window is null)
        {
            return;
        }

        if (checkUpdateResult.Kind is CheckUpdateResultKind.UpdateAvailable)
        {
            ContentDialogResult installUpdateUserConsentResult = await contentDialogFactory
                .CreateForConfirmCancelAsync(
                    SH.FormatViewTitileUpdatePackageAvailableTitle(checkUpdateResult.PackageInformation?.Version),
                    SH.ViewTitileUpdatePackageAvailableContent,
                    ContentDialogButton.Primary)
                .ConfigureAwait(false);

            if (installUpdateUserConsentResult is ContentDialogResult.Primary)
            {
                if (await updateService.LaunchUpdaterAsync().ConfigureAwait(false))
                {
                    taskContext.BeginInvokeOnMainThread(app.Exit);
                }
            }
        }

        await taskContext.SwitchToMainThreadAsync();
    }

    private void NotifyIfDataFolderHasReparsePoint()
    {
        if (new DirectoryInfo(HutaoRuntime.DataFolder).Attributes.HasFlag(FileAttributes.ReparsePoint))
        {
            infoBarService.Warning(SH.FormatViewModelTitleDataFolderHasReparsepoint(HutaoRuntime.DataFolder));
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
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Invert app theme", "TitleViewModel.Command"));
        FrameworkTheming.SetTheme(ThemeHelper.ApplicationToFrameworkInvert(app.RequestedTheme));
    }
#endif
}