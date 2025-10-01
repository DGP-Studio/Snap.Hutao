// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Property;
using Snap.Hutao.Service;
using Snap.Hutao.Service.BackgroundImage;
using Snap.Hutao.UI.Xaml.Control.Theme;
using Snap.Hutao.UI.Xaml.Media.Animation;
using System.Runtime.InteropServices;

namespace Snap.Hutao.ViewModel;

[ConstructorGenerated]
[Service(ServiceLifetime.Transient)]
internal sealed partial class MainViewModel : Abstraction.ViewModel, IDisposable
{
    private readonly WeakReference<Image> weakBackgroundImagePresenter = new(default!);
    private readonly AsyncLock backgroundImageLock = new();

    private readonly IBackgroundImageService backgroundImageService;
    private readonly ITaskContext taskContext;

    private BackgroundImage? previousBackgroundImage;

    public partial AppOptions AppOptions { get; }

    private IObservableProperty<BackgroundImageType>? BackgroundImageTypeCallback { get; set; }

    public override void Dispose()
    {
        using (CriticalSection.Enter())
        {
            Uninitialize();
        }

        base.Dispose();
    }

    public void AttachXamlElement(Image backgroundImagePresenter)
    {
        weakBackgroundImagePresenter.SetTarget(backgroundImagePresenter);
        PrivateUpdateBackgroundAsync(true).SafeForget();
    }

    protected override ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        BackgroundImageTypeCallback = AppOptions.BackgroundImageType.WithValueChangedCallback(static (type, vm) => vm.PrivateUpdateBackgroundAsync(false).SafeForget(), this);
        return ValueTask.FromResult(true);
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
}