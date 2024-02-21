// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Snap.Hutao.Control.Animation;
using Snap.Hutao.Control.Theme;
using Snap.Hutao.Service.BackgroundImage;

namespace Snap.Hutao.ViewModel;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class MainViewModel : Abstraction.ViewModel
{
    private readonly IBackgroundImageService backgroundImageService;
    private readonly ITaskContext taskContext;

    private BackgroundImage? previousBackgroundImage;

    [Command("UpdateBackgroundCommand")]
    private async Task UpdateBackgroundAsync(Image presenter)
    {
        (bool isOk, BackgroundImage backgroundImage) = await backgroundImageService.GetNextBackgroundImageAsync(previousBackgroundImage).ConfigureAwait(false);

        if (isOk)
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
                .StartAsync(presenter)
                .ConfigureAwait(true);

            presenter.Source = backgroundImage.ImageSource;
            double targetOpacity = ThemeHelper.IsDarkMode(presenter.ActualTheme) ? 1 - backgroundImage.Luminance : backgroundImage.Luminance;

            await AnimationBuilder
                .Create()
                .Opacity(
                    to: targetOpacity,
                    duration: ControlAnimationConstants.ImageOpacityFadeInOut,
                    easingType: EasingType.Quartic,
                    easingMode: EasingMode.EaseInOut)
                .StartAsync(presenter)
                .ConfigureAwait(true);
        }
    }
}