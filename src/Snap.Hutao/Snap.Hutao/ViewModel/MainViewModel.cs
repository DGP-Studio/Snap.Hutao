// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Snap.Hutao.Control.Animation;
using Snap.Hutao.Control.Theme;
using Snap.Hutao.Message;
using Snap.Hutao.Service;
using Snap.Hutao.Service.BackgroundImage;
using System.Globalization;

namespace Snap.Hutao.ViewModel;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class MainViewModel : Abstraction.ViewModel, IMainViewModelInitialization, IRecipient<BackgroundImageTypeChangedMessage>
{
    private readonly IBackgroundImageService backgroundImageService;
    private readonly ILogger<MainViewModel> logger;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;

    private BackgroundImage? previousBackgroundImage;
    private Image? backgroundImagePresenter;

    public AppOptions AppOptions { get => appOptions; }

    public void Initialize(IBackgroundImagePresenterAccessor accessor)
    {
        backgroundImagePresenter = accessor.BackgroundImagePresenter;
        UpdateBackgroundAsync(true).SafeForget();
    }

    public void Receive(BackgroundImageTypeChangedMessage message)
    {
        UpdateBackgroundAsync().SafeForget();
    }

    [Command("UpdateBackgroundCommand")]
    private async Task UpdateBackgroundAsync(bool forceRefresh = false)
    {
        if (backgroundImagePresenter is null)
        {
            return;
        }

        (bool shouldRefresh, BackgroundImage? backgroundImage) = await backgroundImageService.GetNextBackgroundImageAsync(forceRefresh ? default : previousBackgroundImage).ConfigureAwait(false);

        if (shouldRefresh)
        {
            previousBackgroundImage = backgroundImage;
            await taskContext.SwitchToMainThreadAsync();

            await AnimationBuilder
                .Create()
                .Opacity(
                    to: 0D,
                    duration: ControlAnimationConstants.ImageOpacityFadeInOut,
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
                backgroundImage?.AccentColor.ToString(CultureInfo.CurrentCulture),
                backgroundImage?.Luminance,
                targetOpacity);

            await AnimationBuilder
                .Create()
                .Opacity(
                    to: targetOpacity,
                    duration: ControlAnimationConstants.ImageOpacityFadeInOut,
                    easingType: EasingType.Quartic,
                    easingMode: EasingMode.EaseInOut)
                .StartAsync(backgroundImagePresenter)
                .ConfigureAwait(true);
        }
    }
}