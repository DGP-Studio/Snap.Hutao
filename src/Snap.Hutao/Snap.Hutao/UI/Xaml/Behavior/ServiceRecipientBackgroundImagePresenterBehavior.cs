// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Animations;
using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Property;
using Snap.Hutao.Service;
using Snap.Hutao.Service.BackgroundImage;
using Snap.Hutao.UI.Content;
using Snap.Hutao.UI.Xaml.Control.Theme;
using Snap.Hutao.UI.Xaml.Media.Animation;
using System.Runtime.InteropServices;

namespace Snap.Hutao.UI.Xaml.Behavior;

internal sealed partial class ServiceRecipientBackgroundImagePresenterBehavior : BehaviorBase<Image>, IDisposable
{
    private readonly CancellationTokenSource unloadCts = new();
    private readonly AsyncLock backgroundImageLock = new();

    private IBackgroundImageService? backgroundImageService;
    private BackgroundImage? previousBackgroundImage;

    private IObservableProperty<BackgroundImageType>? BackgroundImageTypeCallback { get; set; }

    public void Dispose()
    {
        BackgroundImageTypeCallback = null;
        unloadCts.Dispose();
    }

    protected override void OnAssociatedObjectLoaded()
    {
        if (AssociatedObject.XamlRoot.XamlContext()?.ServiceProvider is { } serviceProvider)
        {
            backgroundImageService = serviceProvider.GetRequiredService<IBackgroundImageService>();
            BackgroundImageTypeCallback = serviceProvider.GetRequiredService<AppOptions>().BackgroundImageType
                .WithValueChangedCallback(static (type, behavior) => behavior.PrivateUpdateBackgroundAsync(true, behavior.unloadCts.Token).SafeForget(), this);
            PrivateUpdateBackgroundAsync(false, unloadCts.Token).SafeForget();
        }
    }

    protected override bool Uninitialize()
    {
        unloadCts.Cancel();
        return base.Uninitialize();
    }

    [Command("UpdateBackgroundCommand")]
    private void UpdateBackground()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Update background image", "ServiceRecipientBackgroundImagePresenterBehavior.Command"));
        PrivateUpdateBackgroundAsync(false, unloadCts.Token).SafeForget();
    }

    private async ValueTask PrivateUpdateBackgroundAsync(bool forceRefresh, CancellationToken token = default)
    {
        if (AssociatedObject is not { } backgroundImagePresenter || backgroundImageService is null)
        {
            return;
        }

        ITaskContext taskContext = TaskContext.GetForDependencyObject(backgroundImagePresenter);

        using (await backgroundImageLock.LockAsync().ConfigureAwait(false))
        {
            token.ThrowIfCancellationRequested();
            (bool shouldRefresh, BackgroundImage? backgroundImage) = await backgroundImageService.GetNextBackgroundImageAsync(forceRefresh ? default : previousBackgroundImage, token).ConfigureAwait(false);

            if (shouldRefresh || forceRefresh)
            {
                previousBackgroundImage = backgroundImage;
                await taskContext.SwitchToMainThreadAsync();
                token.ThrowIfCancellationRequested();

                try
                {
                    await AnimationBuilder
                        .Create()
                        .Opacity(
                            to: 0D,
                            duration: Constants.ImageOpacityFadeInOut,
                            easingType: EasingType.Quartic,
                            easingMode: EasingMode.EaseInOut)
                        .StartAsync(backgroundImagePresenter, token)
                        .ConfigureAwait(false);

                    if (XamlApplicationLifetime.Exiting)
                    {
                        return;
                    }

                    await taskContext.SwitchToMainThreadAsync();
                    token.ThrowIfCancellationRequested();
                    backgroundImagePresenter.Source = backgroundImage is null ? null : new BitmapImage(backgroundImage.Path.ToUri());

                    double targetOpacity = backgroundImage is null
                        ? 0
                        : ThemeHelper.IsDarkMode(backgroundImagePresenter.ActualTheme)
                            ? 1 - backgroundImage.Luminance
                            : backgroundImage.Luminance;

                    await AnimationBuilder
                        .Create()
                        .Opacity(
                            to: targetOpacity,
                            duration: Constants.ImageOpacityFadeInOut,
                            easingType: EasingType.Quartic,
                            easingMode: EasingMode.EaseInOut)
                        .StartAsync(backgroundImagePresenter, token)
                        .ConfigureAwait(false);
                }
                catch (COMException)
                {
                    // 0x8001010E The given object has already been closed / disposed and may no longer be used.
                }
            }
        }
    }
}