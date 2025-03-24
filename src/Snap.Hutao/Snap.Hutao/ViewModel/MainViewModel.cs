// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Service;
using Snap.Hutao.Service.BackgroundImage;
using Snap.Hutao.UI.Xaml.Control.Theme;
using Snap.Hutao.UI.Xaml.Media.Animation;

namespace Snap.Hutao.ViewModel;

[ConstructorGenerated]
[Injection(InjectAs.Transient)]
internal sealed partial class MainViewModel : Abstraction.ViewModel
{
    private readonly WeakReference<Image> weakBackgroundImagePresenter = new(default!);
    private readonly AsyncLock backgroundImageLock = new();

    private readonly IBackgroundImageService backgroundImageService;
    private readonly ITaskContext taskContext;

    private BackgroundImage? previousBackgroundImage;

    public partial AppOptions AppOptions { get; }

    public void AttachXamlElement(Image backgroundImagePresenter)
    {
        weakBackgroundImagePresenter.SetTarget(backgroundImagePresenter);
        _ = PrivateUpdateBackgroundAsync(true);
    }

    protected override ValueTask<bool> LoadOverrideAsync()
    {
        AppOptions.PropertyChanged += OnAppOptionsPropertyChanged;
        return ValueTask.FromResult(true);
    }

    protected override void UninitializeOverride()
    {
        AppOptions.PropertyChanged -= OnAppOptionsPropertyChanged;
    }

    private void OnAppOptionsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(AppOptions.BackgroundImageType))
        {
            _ = PrivateUpdateBackgroundAsync(false);
        }
    }

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

                await AnimationBuilder
                    .Create()
                    .Opacity(
                        to: 0D,
                        duration: Constants.ImageOpacityFadeInOut,
                        easingType: EasingType.Quartic,
                        easingMode: EasingMode.EaseInOut)
                    .StartAsync(backgroundImagePresenter)
                    .ConfigureAwait(true);

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
                    .ConfigureAwait(true);
            }
        }
    }
}