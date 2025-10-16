// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Core;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Property;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service;
using Snap.Hutao.Service.BackgroundImage;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.Update;
using Snap.Hutao.UI.Xaml.Behavior.Action;
using Snap.Hutao.UI.Xaml.Control.Theme;
using Snap.Hutao.UI.Xaml.Media.Animation;
using Snap.Hutao.UI.Xaml.View.Window.WebView2;
using System.IO;
using System.Runtime.InteropServices;

namespace Snap.Hutao.ViewModel;

[SuppressMessage("", "SA1201")]
[ConstructorGenerated]
[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Transient)]
internal sealed partial class MainViewModel : Abstraction.ViewModel, IDisposable
{
    // ===== background image fields =====
    private readonly WeakReference<Image> weakBackgroundImagePresenter = new(default!);
    private readonly AsyncLock backgroundImageLock = new();

    private readonly IBackgroundImageService backgroundImageService;

    // ===== title/metadata/update fields (migrated from TitleViewModel) =====
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IMetadataService metadataService;
    private readonly IUpdateService updateService;
    private readonly IMessenger messenger;
    private readonly App app;

    // shared between both original viewmodels
    private readonly ITaskContext taskContext;

    // other services/properties
    public partial AppOptions AppOptions { get; }

    private BackgroundImage? previousBackgroundImage;

    private IObservableProperty<BackgroundImageType>? BackgroundImageTypeCallback { get; set; }

    // Observable property migrated from TitleViewModel
    [ObservableProperty]
    public partial bool IsMetadataInitialized { get; set; }

    // Title property migrated from TitleViewModel
    public static string Title
    {
        get
        {
            string? title = HutaoRuntime.GetDisplayName();
            ArgumentException.ThrowIfNullOrEmpty(title);
            return title;
        }
    }

    // Dispose / lifecycle
    public override void Dispose()
    {
        using (CriticalSection.Enter())
        {
            Uninitialize();
        }

        base.Dispose();
    }

    // Attach background image presenter (migrated)
    public void AttachXamlElement(Image backgroundImagePresenter)
    {
        weakBackgroundImagePresenter.SetTarget(backgroundImagePresenter);
        PrivateUpdateBackgroundAsync(true).SafeForget();
    }

    // Combined LoadOverrideAsync: sets up background callback and runs TitleViewModel init logic
    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        // original MainViewModel behavior
        BackgroundImageTypeCallback = AppOptions.BackgroundImageType.WithValueChangedCallback(static (type, vm) => vm.PrivateUpdateBackgroundAsync(false).SafeForget(), this);

        // migrated TitleViewModel behavior
        ShowUpdateLogWindowAfterUpdate();
        NotifyIfDataFolderHasReparsePoint();
        WaitMetadataInitializationAsync().SafeForget();
        await CheckUpdateAsync().ConfigureAwait(false);

        return true;
    }

    // ===== Background image commands & logic (original) =====

    [Command("UpdateBackgroundCommand")]
    private async Task UpdateBackgroundAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Update background image", "MainViewModel.Command"));
        await PrivateUpdateBackgroundAsync(false).ConfigureAwait(false);
    }

    [SuppressMessage("", "SH003")]
    private async Task PrivateUpdateBackgroundAsync(bool forceRefresh)
    {
        if (!weakBackgroundImagePresenter.TryGetTarget(out Image? backgroundImagePresenter))
        {
            return;
        }

        using (await backgroundImageLock.LockAsync().ConfigureAwait(false))
        {
            (bool shouldRefresh, BackgroundImage? backgroundImage) = await backgroundImageService.GetNextBackgroundImageAsync(forceRefresh ? default : previousBackgroundImage).ConfigureAwait(false);

            if (shouldRefresh)
            {
                previousBackgroundImage = backgroundImage;
                await taskContext.SwitchToMainThreadAsync();

                try
                {
                    await AnimationBuilder
                        .Create()
                        .Opacity(
                            to: 0D,
                            duration: Constants.ImageOpacityFadeInOut,
                            easingType: EasingType.Quartic,
                            easingMode: EasingMode.EaseInOut)
                        .StartAsync(backgroundImagePresenter)
                        .ConfigureAwait(false);

                    if (XamlApplicationLifetime.Exiting)
                    {
                        return;
                    }

                    await taskContext.SwitchToMainThreadAsync();
                    backgroundImagePresenter.Source = backgroundImage is null ? null : new BitmapImage(backgroundImage.Path.ToUri());

                    double targetOpacity = backgroundImage is not null
                        ? ThemeHelper.IsDarkMode(backgroundImagePresenter.ActualTheme)
                            ? 1 - backgroundImage.Luminance
                            : backgroundImage.Luminance
                        : 0;

                    await AnimationBuilder
                        .Create()
                        .Opacity(
                            to: targetOpacity,
                            duration: Constants.ImageOpacityFadeInOut,
                            easingType: EasingType.Quartic,
                            easingMode: EasingMode.EaseInOut)
                        .StartAsync(backgroundImagePresenter)
                        .ConfigureAwait(false);
                }
                catch (COMException)
                {
                    // 0x8001010E The given object has already been closed / disposed and may no longer be used.
                }
            }
        }
    }

    // ===== Title/Metadata/Update logic migrated from TitleViewModel =====

    private void ShowUpdateLogWindowAfterUpdate()
    {
        if (LocalSetting.Get(SettingKeys.AlwaysIsFirstRunAfterUpdate, false) || XamlApplicationLifetime.IsFirstRunAfterUpdate)
        {
            // Check if the window showed, only set to false if it is shown
            if (ShowWebView2WindowAction.TryShow<UpdateLogContentProvider>(currentXamlWindowReference.GetXamlRoot()) is not null)
            {
                SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Show update log window", "MainViewModel.Command"));
                XamlApplicationLifetime.IsFirstRunAfterUpdate = false;
            }
        }
    }

    private async ValueTask CheckUpdateAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Check for update", "MainViewModel.Command"));

        CheckUpdateResult checkUpdateResult = await updateService.CheckUpdateAsync().ConfigureAwait(false);
        await updateService.TriggerUpdateAsync(checkUpdateResult).ConfigureAwait(false);
    }

    private void NotifyIfDataFolderHasReparsePoint()
    {
        if (new DirectoryInfo(HutaoRuntime.DataDirectory).Attributes.HasFlag(FileAttributes.ReparsePoint))
        {
            SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateDebug("Data folder has reparse point", "MainViewModel.Command"));
            messenger.Send(InfoBarMessage.Warning(SH.FormatViewModelTitleDataFolderHasReparsepoint(HutaoRuntime.DataDirectory)));
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
            SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Metadata initialized", "MainViewModel.Command"));
        }
    }

    // ===== Debug-only command migrated from TitleViewModel =====

#if DEBUG
    [Command("InvertAppThemeCommand")]
    private void InvertAppTheme()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Invert app theme", "MainViewModel.Command"));
        UI.Xaml.FrameworkTheming.SetTheme(UI.Xaml.Control.Theme.ThemeHelper.ApplicationToFrameworkInvert(app.RequestedTheme));
    }
#endif
}
