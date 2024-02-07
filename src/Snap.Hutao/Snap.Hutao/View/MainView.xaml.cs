// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Snap.Hutao.Control.Theme;
using Snap.Hutao.Service.BackgroundImage;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.View.Page;

namespace Snap.Hutao.View;

/// <summary>
/// 主视图
/// </summary>
[HighQuality]
internal sealed partial class MainView : UserControl
{
    private readonly INavigationService navigationService;
    private readonly IBackgroundImageService backgroundImageService;
    private TaskCompletionSource acutalThemeChangedTaskCompletionSource = new();
    private CancellationTokenSource periodicTimerCancellationTokenSource = new();
    private BackgroundImage? previousBackgroundImage;

    /// <summary>
    /// 构造一个新的主视图
    /// </summary>
    public MainView()
    {
        InitializeComponent();

        ActualThemeChanged += OnActualThemeChanged;

        IServiceProvider serviceProvider = Ioc.Default;

        backgroundImageService = serviceProvider.GetRequiredService<IBackgroundImageService>();
        RunBackgroundImageLoopAsync(serviceProvider.GetRequiredService<ITaskContext>()).SafeForget();

        navigationService = serviceProvider.GetRequiredService<INavigationService>();
        navigationService
            .As<INavigationInitialization>()?
            .Initialize(NavView, ContentFrame);

        navigationService.Navigate<AnnouncementPage>(INavigationAwaiter.Default, true);
    }

    private async ValueTask RunBackgroundImageLoopAsync(ITaskContext taskContext)
    {
        using (PeriodicTimer timer = new(TimeSpan.FromMinutes(5)))
        {
            do
            {
                (bool isOk, BackgroundImage backgroundImage) = await backgroundImageService.GetNextBackgroundImageAsync(previousBackgroundImage).ConfigureAwait(false);

                if (isOk)
                {
                    previousBackgroundImage = backgroundImage;
                    await taskContext.SwitchToMainThreadAsync();

                    await AnimationBuilder
                        .Create()
                        .Opacity(to: 0D, duration: TimeSpan.FromMilliseconds(1000), easingType: EasingType.Sine, easingMode: EasingMode.EaseIn)
                        .StartAsync(BackdroundImagePresenter)
                        .ConfigureAwait(true);

                    BackdroundImagePresenter.Source = backgroundImage.ImageSource;
                    double targetOpacity = ThemeHelper.IsDarkMode(ActualTheme) ? 1 - backgroundImage.Luminance : backgroundImage.Luminance;

                    await AnimationBuilder
                        .Create()
                        .Opacity(to: targetOpacity, duration: TimeSpan.FromMilliseconds(1000), easingType: EasingType.Sine, easingMode: EasingMode.EaseOut)
                        .StartAsync(BackdroundImagePresenter)
                        .ConfigureAwait(true);
                }

                try
                {
                    await Task.WhenAny(timer.WaitForNextTickAsync(periodicTimerCancellationTokenSource.Token).AsTask(), acutalThemeChangedTaskCompletionSource.Task).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }

                acutalThemeChangedTaskCompletionSource = new();
                periodicTimerCancellationTokenSource = new();
            }
            while (true);
        }
    }

    private void OnActualThemeChanged(FrameworkElement frameworkElement, object args)
    {
        acutalThemeChangedTaskCompletionSource.TrySetResult();
        periodicTimerCancellationTokenSource.Cancel();
    }
}