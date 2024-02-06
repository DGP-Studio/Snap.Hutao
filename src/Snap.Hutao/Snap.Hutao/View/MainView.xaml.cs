// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Xaml.Controls;
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

    /// <summary>
    /// 构造一个新的主视图
    /// </summary>
    public MainView()
    {
        InitializeComponent();

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
                (bool isOk, BackgroundImage backgroundImage) = await backgroundImageService.GetNextBackgroundImageAsync().ConfigureAwait(false);

                if (isOk)
                {
                    await taskContext.SwitchToMainThreadAsync();

                    await AnimationBuilder
                        .Create()
                        .Opacity(to: 0D, duration: TimeSpan.FromMilliseconds(300), easingType: EasingType.Sine)
                        .StartAsync(BackdroundImagePresenter)
                        .ConfigureAwait(true);

                    BackdroundImagePresenter.Source = backgroundImage.ImageSource;
                    double targetOpacity = (1 - backgroundImage.Luminance) * 0.8;

                    await AnimationBuilder
                        .Create()
                        .Opacity(to: targetOpacity, duration: TimeSpan.FromMilliseconds(300), easingType: EasingType.Sine)
                        .StartAsync(BackdroundImagePresenter)
                        .ConfigureAwait(true);
                }
            } while (await timer.WaitForNextTickAsync().ConfigureAwait(false));
        }
    }
}