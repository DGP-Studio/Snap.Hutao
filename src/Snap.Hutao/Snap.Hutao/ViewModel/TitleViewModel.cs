// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.Update;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.UI.Xaml.Behavior.Action;
using Snap.Hutao.UI.Xaml.Control.Theme;
using Snap.Hutao.UI.Xaml.View.Window.WebView2;
using System.IO;

namespace Snap.Hutao.ViewModel;

[ConstructorGenerated]
[Injection(InjectAs.Transient)]
[SuppressMessage("", "SA1201")]
internal sealed partial class TitleViewModel : Abstraction.ViewModel
{
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IMetadataService metadataService;
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

    public bool IsMetadataInitialized { get; set => SetProperty(ref field, value); }

    protected override async ValueTask<bool> LoadOverrideAsync()
    {
        ShowUpdateLogWindowAfterUpdate();
        NotifyIfDataFolderHasReparsePoint();
        WaitMetadataInitializationAsync().SafeForget();
        await CheckUpdateAsync().ConfigureAwait(false);
        return true;
    }

    private void ShowUpdateLogWindowAfterUpdate()
    {
        if (LocalSetting.Get(SettingKeys.AlwaysIsFirstRunAfterUpdate, false) || XamlApplicationLifetime.IsFirstRunAfterUpdate)
        {
            // Check if the window showed, only set to false if it is shown
            if (ShowWebView2WindowAction.TryShow<UpdateLogContentProvider>(currentXamlWindowReference.GetXamlRoot()) is not null)
            {
                SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Show update log window", "TitleViewModel.Command"));
                XamlApplicationLifetime.IsFirstRunAfterUpdate = false;
            }
        }
    }

    private async ValueTask CheckUpdateAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Check for update", "TitleViewModel.Command"));

        CheckUpdateResult checkUpdateResult = await updateService.CheckUpdateAsync().ConfigureAwait(false);

        if (checkUpdateResult.Kind is CheckUpdateResultKind.UpdateAvailable)
        {
            if (currentXamlWindowReference.Window is null)
            {
                return;
            }

            ContentDialogResult installUpdateUserConsentResult = await contentDialogFactory
                .CreateForConfirmCancelAsync(
                    SH.FormatViewTitileUpdatePackageAvailableTitle(checkUpdateResult.PackageInformation?.Version),
                    SH.ViewTitileUpdatePackageAvailableContent,
                    ContentDialogButton.Primary)
                .ConfigureAwait(false);

            if (installUpdateUserConsentResult is ContentDialogResult.Primary)
            {
                if (await updateService.LaunchUpdaterAsync().ConfigureAwait(false) is (false, { } ex))
                {
                    infoBarService.Error(ex);
                }
            }
        }
    }

    private void NotifyIfDataFolderHasReparsePoint()
    {
        if (new DirectoryInfo(HutaoRuntime.DataFolder).Attributes.HasFlag(FileAttributes.ReparsePoint))
        {
            SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateDebug("Data folder has reparse point", "TitleViewModel.Command"));
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
            SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Metadata initialized", "TitleViewModel.Command"));
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