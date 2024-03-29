// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Win32;
using System.IO;
using Windows.Foundation;

namespace Snap.Hutao.Control.Image.Implementation;

[SuppressMessage("", "CA1001")]
[SuppressMessage("", "SH003")]
[TemplateVisualState(Name = LoadingState, GroupName = CommonGroup)]
[TemplateVisualState(Name = LoadedState, GroupName = CommonGroup)]
[TemplateVisualState(Name = UnloadedState, GroupName = CommonGroup)]
[TemplateVisualState(Name = FailedState, GroupName = CommonGroup)]
[TemplatePart(Name = PartImage, Type = typeof(object))]
[TemplatePart(Name = PartPlaceholderImage, Type = typeof(object))]
[DependencyProperty("Stretch", typeof(Stretch), Stretch.Uniform)]
[DependencyProperty("DecodePixelHeight", typeof(int), 0)]
[DependencyProperty("DecodePixelWidth", typeof(int), 0)]
[DependencyProperty("DecodePixelType", typeof(DecodePixelType), DecodePixelType.Physical)]
[DependencyProperty("IsCacheEnabled", typeof(bool), false)]
[DependencyProperty("EnableLazyLoading", typeof(bool), false, nameof(EnableLazyLoadingChanged))]
[DependencyProperty("LazyLoadingThreshold", typeof(double), default(double), nameof(LazyLoadingThresholdChanged))]
[DependencyProperty("PlaceholderSource", typeof(object), default(object))]
[DependencyProperty("PlaceholderStretch", typeof(Stretch), Stretch.Uniform)]
[DependencyProperty("PlaceholderMargin", typeof(Thickness))]
[DependencyProperty("Source", typeof(object), default(object), nameof(SourceChanged))]
internal abstract partial class ImageExBase : Microsoft.UI.Xaml.Controls.Control, IAlphaMaskProvider
{
    protected const string PartImage = "Image";
    protected const string PartPlaceholderImage = "PlaceholderImage";
    protected const string CommonGroup = "CommonStates";
    protected const string LoadingState = "Loading";
    protected const string LoadedState = "Loaded";
    protected const string UnloadedState = "Unloaded";
    protected const string FailedState = "Failed";

    private CancellationTokenSource? tokenSource;
    private object? lazyLoadingSource;
    private bool isInViewport;

    public bool IsInitialized { get; private set; }

    public bool WaitUntilLoaded
    {
        get => true;
    }

    protected object? Image { get; private set; }

    protected object? PlaceholderImage { get; private set; }

    public abstract CompositionBrush GetAlphaMask();

    protected virtual Task<ImageSource?> ProvideCachedResourceAsync(Uri imageUri, CancellationToken token)
    {
        // By default we just use the built-in UWP image cache provided within the Image control.
        return Task.FromResult<ImageSource?>(new BitmapImage(imageUri));
    }

    protected virtual void OnImageOpened(object sender, RoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, LoadedState, true);
    }

    protected virtual void OnImageFailed(object sender, ExceptionRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, FailedState, true);
    }

    protected override void OnApplyTemplate()
    {
        RemoveImageOpened(OnImageOpened);
        RemoveImageFailed(OnImageFailed);

        Image = GetTemplateChild(PartImage);
        PlaceholderImage = GetTemplateChild(PartPlaceholderImage);

        IsInitialized = true;

        if (Source is null || !EnableLazyLoading || isInViewport)
        {
            lazyLoadingSource = null;
            SetSource(Source);
        }
        else
        {
            lazyLoadingSource = Source;
        }

        AttachImageOpened(OnImageOpened);
        AttachImageFailed(OnImageFailed);

        base.OnApplyTemplate();

        void AttachImageOpened(RoutedEventHandler handler)
        {
            if (Image is Microsoft.UI.Xaml.Controls.Image image)
            {
                image.ImageOpened += handler;
            }
            else if (Image is ImageBrush brush)
            {
                brush.ImageOpened += handler;
            }
        }

        void AttachImageFailed(ExceptionRoutedEventHandler handler)
        {
            if (Image is Microsoft.UI.Xaml.Controls.Image image)
            {
                image.ImageFailed += handler;
            }
            else if (Image is ImageBrush brush)
            {
                brush.ImageFailed += handler;
            }
        }

        void RemoveImageOpened(RoutedEventHandler handler)
        {
            if (Image is Microsoft.UI.Xaml.Controls.Image image)
            {
                image.ImageOpened -= handler;
            }
            else if (Image is ImageBrush brush)
            {
                brush.ImageOpened -= handler;
            }
        }

        void RemoveImageFailed(ExceptionRoutedEventHandler handler)
        {
            if (Image is Microsoft.UI.Xaml.Controls.Image image)
            {
                image.ImageFailed -= handler;
            }
            else if (Image is ImageBrush brush)
            {
                brush.ImageFailed -= handler;
            }
        }
    }

    private static void EnableLazyLoadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageExBase control)
        {
            return;
        }

        bool value = (bool)e.NewValue;
        if (value)
        {
            control.LayoutUpdated += control.OnImageExBaseLayoutUpdated;
            control.InvalidateLazyLoading();
        }
        else
        {
            control.LayoutUpdated -= control.OnImageExBaseLayoutUpdated;
        }
    }

    private static void LazyLoadingThresholdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ImageExBase { EnableLazyLoading: true } control)
        {
            control.InvalidateLazyLoading();
        }
    }

    private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageExBase control)
        {
            return;
        }

        if (e.OldValue is not null && e.NewValue is not null && e.OldValue.Equals(e.NewValue))
        {
            return;
        }

        if (e.NewValue is null || !control.EnableLazyLoading || control.isInViewport)
        {
            control.lazyLoadingSource = null;
            control.SetSource(e.NewValue);
        }
        else
        {
            control.lazyLoadingSource = e.NewValue;
        }
    }

    private static bool IsHttpUri(Uri uri)
    {
        return uri.IsAbsoluteUri && (uri.Scheme == "http" || uri.Scheme == "https");
    }

    private void AttachSource(ImageSource? source)
    {
        // Setting the source at this point should call ImageExOpened/VisualStateManager.GoToState
        // as we register to both the ImageOpened/ImageFailed events of the underlying control.
        // We only need to call those methods if we fail in other cases before we get here.
        if (Image is Microsoft.UI.Xaml.Controls.Image image)
        {
            image.Source = source;
        }
        else if (Image is ImageBrush brush)
        {
            brush.ImageSource = source;
        }

        if (source is null)
        {
            VisualStateManager.GoToState(this, UnloadedState, true);
        }
        else if (source is BitmapSource { PixelHeight: > 0, PixelWidth: > 0 })
        {
            VisualStateManager.GoToState(this, LoadedState, true);
        }
    }

    private void AttachPlaceholderSource(ImageSource? source)
    {
        if (PlaceholderImage is Microsoft.UI.Xaml.Controls.Image image)
        {
            image.Source = source;
        }
        else if (PlaceholderImage is ImageBrush brush)
        {
            brush.ImageSource = source;
        }

        if (source is null)
        {
            VisualStateManager.GoToState(this, UnloadedState, true);
        }
        else if (source is BitmapSource { PixelHeight: > 0, PixelWidth: > 0 })
        {
            VisualStateManager.GoToState(this, LoadedState, true);
        }
    }

    private async void SetSource(object? source)
    {
        if (!IsInitialized)
        {
            return;
        }

        tokenSource?.Cancel();

        tokenSource = new CancellationTokenSource();

        AttachSource(null);

        if (source is null)
        {
            return;
        }

        VisualStateManager.GoToState(this, LoadingState, true);

        if (source as ImageSource is { } imageSource)
        {
            AttachSource(imageSource);

            return;
        }

        if (source as Uri is not { } uri)
        {
            string? url = source as string ?? source.ToString();
            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
            {
                VisualStateManager.GoToState(this, FailedState, true);
                return;
            }
        }

        if (!IsHttpUri(uri) && !uri.IsAbsoluteUri)
        {
            uri = new Uri("ms-appx:///" + uri.OriginalString.TrimStart('/'));
        }

        try
        {
            await LoadImageAsync(uri, tokenSource.Token).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            SetPlaceholderSource(PlaceholderSource);

            if (ex is OperationCanceledException)
            {
                // nothing to do as cancellation has been requested.
            }
            else
            {
                VisualStateManager.GoToState(this, FailedState, true);
            }
        }
    }

    private async void SetPlaceholderSource(object? source)
    {
        if (!IsInitialized)
        {
            return;
        }

        tokenSource?.Cancel();
        tokenSource = new();

        AttachPlaceholderSource(null);

        if (source is null)
        {
            return;
        }

        if (source as ImageSource is { } imageSource)
        {
            AttachPlaceholderSource(imageSource);

            return;
        }

        if (source as Uri is not { } uri)
        {
            string? url = source as string ?? source.ToString();
            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
            {
                return;
            }
        }

        if (!IsHttpUri(uri) && !uri.IsAbsoluteUri)
        {
            uri = new Uri("ms-appx:///" + uri.OriginalString.TrimStart('/'));
        }

        try
        {
            if (uri is null)
            {
                return;
            }

            ImageSource? img = await ProvideCachedResourceAsync(uri, tokenSource.Token).ConfigureAwait(true);

            ArgumentNullException.ThrowIfNull(tokenSource);
            if (!tokenSource.IsCancellationRequested)
            {
                // Only attach our image if we still have a valid request.
                AttachPlaceholderSource(img);
            }
        }
        catch (OperationCanceledException)
        {
            // nothing to do as cancellation has been requested.
        }
        catch
        {
        }
    }

    private async Task LoadImageAsync(Uri imageUri, CancellationToken token)
    {
        if (imageUri is null)
        {
            return;
        }

        if (IsCacheEnabled)
        {
            ImageSource? img = await ProvideCachedResourceAsync(imageUri, token).ConfigureAwait(true);

            ArgumentNullException.ThrowIfNull(tokenSource);
            if (!tokenSource.IsCancellationRequested)
            {
                // Only attach our image if we still have a valid request.
                AttachSource(img);
            }
        }
        else if (string.Equals(imageUri.Scheme, "data", StringComparison.OrdinalIgnoreCase))
        {
            string source = imageUri.OriginalString;
            const string base64Head = "base64,";
            int index = source.IndexOf(base64Head, StringComparison.Ordinal);
            if (index >= 0)
            {
                byte[] bytes = Convert.FromBase64String(source[(index + base64Head.Length)..]);
                BitmapImage bitmap = new();
                await bitmap.SetSourceAsync(new MemoryStream(bytes).AsRandomAccessStream());

                ArgumentNullException.ThrowIfNull(tokenSource);
                if (!tokenSource.IsCancellationRequested)
                {
                    AttachSource(bitmap);
                }
            }
        }
        else
        {
            AttachSource(new BitmapImage(imageUri)
            {
                CreateOptions = BitmapCreateOptions.IgnoreImageCache,
            });
        }
    }

    private void OnImageExBaseLayoutUpdated(object? sender, object e)
    {
        InvalidateLazyLoading();
    }

    private void InvalidateLazyLoading()
    {
        if (!IsLoaded)
        {
            isInViewport = false;
            return;
        }

        // Find the first ascendant ScrollViewer, if not found, use the root element.
        FrameworkElement? hostElement = default;
        IEnumerable<FrameworkElement> ascendants = this.FindAscendants().OfType<FrameworkElement>();
        foreach (FrameworkElement ascendant in ascendants)
        {
            hostElement = ascendant;
            if (hostElement is Microsoft.UI.Xaml.Controls.ScrollViewer)
            {
                break;
            }
        }

        if (hostElement is null)
        {
            isInViewport = false;
            return;
        }

        Rect controlRect = TransformToVisual(hostElement).TransformBounds(StructMarshal.Rect(ActualSize));
        double lazyLoadingThreshold = LazyLoadingThreshold;

        // Left/Top 1 Threshold, Right/Bottom 2 Threshold
        Rect hostRect = new(
            0 - lazyLoadingThreshold,
            0 - lazyLoadingThreshold,
            hostElement.ActualWidth + (2 * lazyLoadingThreshold),
            hostElement.ActualHeight + (2 * lazyLoadingThreshold));

        if (controlRect.IntersectsWith(hostRect))
        {
            isInViewport = true;

            if (lazyLoadingSource is not null)
            {
                object source = lazyLoadingSource;
                lazyLoadingSource = null;
                SetSource(source);
            }
        }
        else
        {
            isInViewport = false;
        }
    }
}