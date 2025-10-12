// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.DataTransfer;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.UI.Content;
using Snap.Hutao.UI.Xaml.Control.Theme;
using Snap.Hutao.UI.Xaml.Media.Animation;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace Snap.Hutao.UI.Xaml.Control.Image;

[SuppressMessage("", "CA1001")]
[SuppressMessage("", "SH003")]
[TemplatePart(Name = PartImage, Type = typeof(Microsoft.UI.Xaml.Controls.Image))]
[DependencyProperty<string>("SourceName", DefaultValue = "UnknownSourceName")]
[DependencyProperty<string>("CachedName", DefaultValue = "UnknownCachedName")]
[DependencyProperty<Thickness>("NineGrid", NotNull = true)]
[DependencyProperty<Stretch>("Stretch", DefaultValue = Stretch.Uniform, NotNull = true)]
[DependencyProperty<object>("PlaceholderSource")]
[DependencyProperty<object>("Source", PropertyChangedCallbackName = nameof(OnSourceChanged))]
[DependencyProperty<bool>("ShowAsMonoChrome", DefaultValue = false, NotNull = true)]
internal sealed partial class CachedImage : Microsoft.UI.Xaml.Controls.Control
{
    private const string PartImage = "Image";

    private static readonly ConditionalWeakTable<Microsoft.UI.Xaml.Controls.Image, object> IsPlaceholder = [];

    private readonly AsyncLock sourceLock = new();

    private CancellationTokenSource? sourceCts;

    public CachedImage()
    {
        DefaultStyleKey = typeof(CachedImage);
        Loaded += OnLoaded;
    }

    private Microsoft.UI.Xaml.Controls.Image? Image { get; set; }

    private bool IsInitialized { get; set; }

    protected override void OnApplyTemplate()
    {
        if (Image is { } oldImage)
        {
            oldImage.ImageOpened -= OnImageOpened;
            oldImage.ImageFailed -= OnImageFailed;
        }

        Image = GetTemplateChild(PartImage) as Microsoft.UI.Xaml.Controls.Image;

        if (Image is { } newImage)
        {
            newImage.ImageOpened += OnImageOpened;
            newImage.ImageFailed += OnImageFailed;
        }

        IsInitialized = true;
        SetSourceAsync(Source, false).SafeForget();

        base.OnApplyTemplate();
    }

    private static bool IsHttpUri(Uri uri)
    {
        return uri is { IsAbsoluteUri: true, Scheme: "http" or "https" };
    }

    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not CachedImage control)
        {
            return;
        }

        if (Equals(e.OldValue, e.NewValue))
        {
            return;
        }

        control.SetSourceAsync(e.NewValue, false).SafeForget();
    }

    private static void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        if (sender is not CachedImage control)
        {
            return;
        }

        control.SetSourceAsync(control.Source, false).SafeForget();
    }

    private static void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not CachedImage control)
        {
            return;
        }

        control.ActualThemeChanged += OnActualThemeChanged;
        control.Unloaded += OnUnloaded;
    }

    private static void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (sender is not CachedImage control)
        {
            return;
        }

        control.ActualThemeChanged -= OnActualThemeChanged;
        control.Loaded -= OnLoaded;
        control.Unloaded -= OnUnloaded;
    }

    private static void OnImageOpened(object sender, RoutedEventArgs e)
    {
        if (sender is not UIElement element)
        {
            return;
        }

        AnimationBuilder
            .Create()
            .Opacity(
                to: 1D,
                duration: Constants.ImageOpacityFadeInOutFast,
                easingType: EasingType.Quartic,
                easingMode: EasingMode.EaseInOut)
            .Start(element);
    }

    private static void OnImageFailed(object sender, ExceptionRoutedEventArgs e)
    {
        if (sender is not Microsoft.UI.Xaml.Controls.Image image)
        {
            return;
        }

        if (IsPlaceholder.TryGetValue(image, out _))
        {
            return;
        }

        AnimationBuilder
            .Create()
            .Opacity(
                to: 0,
                duration: Constants.ImageOpacityFadeInOutFast,
                easingType: EasingType.Quartic,
                easingMode: EasingMode.EaseInOut)
            .Start(image);
    }

    private static async Task ImageAttachUriSourceWithAnimationAsync(Microsoft.UI.Xaml.Controls.Image? target, Uri? uri, bool placeholder)
    {
        if (target is null)
        {
            return;
        }

        if (placeholder)
        {
            IsPlaceholder.Add(target, new());
        }
        else
        {
            IsPlaceholder.Remove(target);
        }

        if (uri is null)
        {
            await AnimationBuilder
                .Create()
                .Opacity(
                    to: 0D,
                    duration: Constants.ImageOpacityFadeInOutFast,
                    easingType: EasingType.Quartic,
                    easingMode: EasingMode.EaseInOut)
                .StartAsync(target)
                .ConfigureAwait(true);

            if (!target.IsLoaded)
            {
                return;
            }

            target.Visibility = Visibility.Collapsed;
            target.Source = null;
        }
        else
        {
            // https://learn.microsoft.com/en-us/windows/uwp/debug-test-perf/optimize-animations-and-media#optimize-image-resources
            // Connecting the BitmapImage to the tree before setting its UriSource
            target.Visibility = Visibility.Visible;

            BitmapImage source = new();

            // https://learn.microsoft.com/en-us/windows/uwp/debug-test-perf/optimize-animations-and-media#right-sized-decoding
            // UriSource should be set after the Image is connected to live XAML tree
            target.Source = source;
            source.DecodePixelType = DecodePixelType.Logical;
            source.UriSource = uri;
        }
    }

    private async Task<Uri?> ProvideCachedResourceAsync(Uri imageUri, CancellationToken token)
    {
        if (XamlRoot.XamlContext()?.ServiceProvider.GetRequiredService<IImageCache>() is not { } imageCache)
        {
            return default;
        }

        try
        {
            HutaoException.ThrowIf(string.IsNullOrEmpty(imageUri.Host), SH.ControlImageCachedImageInvalidResourceUri);
            ElementTheme theme = ShowAsMonoChrome ? ThemeHelper.ApplicationToElement(ThemeHelper.ElementToApplication(ActualTheme)) : ElementTheme.Default;
            string file = await imageCache.GetFileFromCacheAsync(imageUri, theme).ConfigureAwait(true);
            token.ThrowIfCancellationRequested();
            CachedName = Path.GetFileName(file);
            SourceName = Path.GetFileName(imageUri.ToString());
            return file.ToUri();
        }
        catch (OperationCanceledException)
        {
            // Ignored
            throw;
        }
        catch (InternalImageCacheException)
        {
            // Ignored
            throw;
        }
        catch (COMException)
        {
            try
            {
                // The image is corrupted, remove it.
                imageCache.Remove(imageUri);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }

            return default;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            return default;
        }
    }

    private async Task SetSourceAsync(object? source, bool placeholder)
    {
        try
        {
            if (!IsInitialized)
            {
                return;
            }

            if (sourceCts is not null)
            {
                await sourceCts.CancelAsync().ConfigureAwait(true);
                sourceCts.Dispose();
            }

            sourceCts = new();
            CancellationToken token = sourceCts.Token;

            try
            {
                using (await sourceLock.LockAsync().ConfigureAwait(true))
                {
                    // Remove old ImageSource from tree
                    await ImageAttachUriSourceWithAnimationAsync(Image, default, placeholder).ConfigureAwait(true);

                    if (source is null)
                    {
                        return;
                    }

                    if (source as Uri is not { } uri)
                    {
                        if (!Uri.TryCreate(source as string ?? source.ToString(), UriKind.RelativeOrAbsolute, out uri))
                        {
                            await SetSourceAsync(PlaceholderSource, true).ConfigureAwait(true);
                            return;
                        }
                    }

                    if (!IsHttpUri(uri) && !uri.IsAbsoluteUri)
                    {
                        uri = new("ms-appx:///" + uri.OriginalString.TrimStart('/'));
                    }

                    Uri? targetUri = uri.Scheme is "ms-appx" ? uri : await ProvideCachedResourceAsync(uri, token).ConfigureAwait(true);
                    if (!token.IsCancellationRequested)
                    {
                        // Only attach our image if we still have a valid request.
                        await ImageAttachUriSourceWithAnimationAsync(Image, targetUri, placeholder).ConfigureAwait(true);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Ignored
            }
            catch
            {
                if (!IsLoaded)
                {
                    return;
                }

                if (!placeholder)
                {
                    await SetSourceAsync(PlaceholderSource, true).ConfigureAwait(true);
                }
            }
        }
        catch (COMException ex)
        {
            if (ex.HResult != unchecked((int)0x8000FFFF))
            {
                throw;
            }
        }
    }

    [Command("CopyToClipboardCommand")]
    private async Task CopyToClipboard()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Copy image to Clipboard", "CachedImage.Command", [("source_name", SourceName ?? "<null>")]));

        if (XamlRoot.XamlContext()?.ServiceProvider.GetRequiredService<IClipboardProvider>() is not { } clipboardProvider)
        {
            return;
        }

        if (Image is { Source: BitmapImage bitmap })
        {
            using (FileStream netStream = File.OpenRead(bitmap.UriSource.LocalPath))
            {
                using (IRandomAccessStream fxStream = netStream.AsRandomAccessStream())
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fxStream);
                    SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                    using (InMemoryRandomAccessStream memory = new())
                    {
                        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, memory);
                        encoder.SetSoftwareBitmap(softwareBitmap);
                        await encoder.FlushAsync();
                        await clipboardProvider.SetBitmapAsync(memory).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}