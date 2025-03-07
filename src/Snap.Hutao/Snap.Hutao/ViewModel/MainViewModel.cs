// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Service;
using Snap.Hutao.Service.BackgroundImage;
using Snap.Hutao.UI.Xaml.Control.Theme;
using Snap.Hutao.UI.Xaml.Media.Animation;

namespace Snap.Hutao.ViewModel;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class MainViewModel : Abstraction.ViewModel, IMainViewModelInitialization
{
    private readonly AsyncLock backgroundImageLock = new();

    private readonly IBackgroundImageService backgroundImageService;
    private readonly ILogger<MainViewModel> logger;
    private readonly ITaskContext taskContext;

    private BackgroundImage? previousBackgroundImage;
    private Image? backgroundImagePresenter;

    public partial AppOptions AppOptions { get; }

    public void Initialize(IBackgroundImagePresenterAccessor accessor)
    {
        backgroundImagePresenter = accessor.BackgroundImagePresenter;
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
        if (backgroundImagePresenter is null)
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

                backgroundImagePresenter.Source = backgroundImage?.ImageSource;
                double targetOpacity = backgroundImage is not null
                    ? ThemeHelper.IsDarkMode(backgroundImagePresenter.ActualTheme)
                        ? 1 - backgroundImage.Luminance
                        : backgroundImage.Luminance
                    : 0;

                logger.LogInformation(
                    "Background image: [Accent color: {AccentColor}] [Luminance: {Luminance}] [Opacity: {TargetOpacity}]",
                    backgroundImage?.AccentColor,
                    backgroundImage?.Luminance,
                    targetOpacity);

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