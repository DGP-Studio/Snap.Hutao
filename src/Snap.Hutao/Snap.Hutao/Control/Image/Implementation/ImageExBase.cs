// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System.IO;
using Windows.Foundation;

namespace Snap.Hutao.Control.Image.Implementation;

internal delegate void ImageExFailedEventHandler(object sender, ImageExFailedEventArgs e);

internal delegate void ImageExOpenedEventHandler(object sender, ImageExOpenedEventArgs e);

[SuppressMessage("", "CA1001")]
[SuppressMessage("", "SH003")]
[TemplateVisualState(Name = LoadingState, GroupName = CommonGroup)]
[TemplateVisualState(Name = LoadedState, GroupName = CommonGroup)]
[TemplateVisualState(Name = UnloadedState, GroupName = CommonGroup)]
[TemplateVisualState(Name = FailedState, GroupName = CommonGroup)]
[TemplatePart(Name = PartImage, Type = typeof(object))]
internal abstract class ImageExBase : Microsoft.UI.Xaml.Controls.Control, IAlphaMaskProvider
{
    protected const string PartImage = "Image";
    protected const string CommonGroup = "CommonStates";
    protected const string LoadingState = "Loading";
    protected const string LoadedState = "Loaded";
    protected const string UnloadedState = "Unloaded";
    protected const string FailedState = "Failed";

    private static readonly DependencyProperty StretchProperty = DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(ImageExBase), new PropertyMetadata(Stretch.Uniform));
    private static readonly DependencyProperty DecodePixelHeightProperty = DependencyProperty.Register(nameof(DecodePixelHeight), typeof(int), typeof(ImageExBase), new PropertyMetadata(0));
    private static readonly DependencyProperty DecodePixelTypeProperty = DependencyProperty.Register(nameof(DecodePixelType), typeof(int), typeof(ImageExBase), new PropertyMetadata(DecodePixelType.Physical));
    private static readonly DependencyProperty DecodePixelWidthProperty = DependencyProperty.Register(nameof(DecodePixelWidth), typeof(int), typeof(ImageExBase), new PropertyMetadata(0));
    private static readonly DependencyProperty IsCacheEnabledProperty = DependencyProperty.Register(nameof(IsCacheEnabled), typeof(bool), typeof(ImageExBase), new PropertyMetadata(false));
    private static readonly DependencyProperty EnableLazyLoadingProperty = DependencyProperty.Register(nameof(EnableLazyLoading), typeof(bool), typeof(ImageExBase), new PropertyMetadata(false, EnableLazyLoadingChanged));
    private static readonly DependencyProperty LazyLoadingThresholdProperty = DependencyProperty.Register(nameof(LazyLoadingThreshold), typeof(double), typeof(ImageExBase), new PropertyMetadata(default(double), LazyLoadingThresholdChanged));
    private static readonly DependencyProperty PlaceholderSourceProperty = DependencyProperty.Register(nameof(PlaceholderSource), typeof(ImageSource), typeof(ImageExBase), new PropertyMetadata(default(ImageSource), PlaceholderSourceChanged));
    private static readonly DependencyProperty PlaceholderStretchProperty = DependencyProperty.Register(nameof(PlaceholderStretch), typeof(Stretch), typeof(ImageExBase), new PropertyMetadata(default(Stretch)));
    private static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(object), typeof(ImageExBase), new PropertyMetadata(null, SourceChanged));

    private CancellationTokenSource? tokenSource;
    private object? lazyLoadingSource;
    private bool isInViewport;

    public event ImageExFailedEventHandler? ImageExFailed;

    public event ImageExOpenedEventHandler? ImageExOpened;

    public event EventHandler? ImageExInitialized;

    public bool IsInitialized { get; private set; }

    public int DecodePixelHeight
    {
        get => (int)GetValue(DecodePixelHeightProperty);
        set => SetValue(DecodePixelHeightProperty, value);
    }

    public DecodePixelType DecodePixelType
    {
        get => (DecodePixelType)GetValue(DecodePixelTypeProperty);
        set => SetValue(DecodePixelTypeProperty, value);
    }

    public int DecodePixelWidth
    {
        get => (int)GetValue(DecodePixelWidthProperty);
        set => SetValue(DecodePixelWidthProperty, value);
    }

    public Stretch Stretch
    {
        get => (Stretch)GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    public bool IsCacheEnabled
    {
        get => (bool)GetValue(IsCacheEnabledProperty);
        set => SetValue(IsCacheEnabledProperty, value);
    }

    public bool EnableLazyLoading
    {
        get => (bool)GetValue(EnableLazyLoadingProperty);
        set => SetValue(EnableLazyLoadingProperty, value);
    }

    public double LazyLoadingThreshold
    {
        get => (double)GetValue(LazyLoadingThresholdProperty);
        set => SetValue(LazyLoadingThresholdProperty, value);
    }

    public ImageSource PlaceholderSource
    {
        get => (ImageSource)GetValue(PlaceholderSourceProperty);
        set => SetValue(PlaceholderSourceProperty, value);
    }

    public Stretch PlaceholderStretch
    {
        get => (Stretch)GetValue(PlaceholderStretchProperty);
        set => SetValue(PlaceholderStretchProperty, value);
    }

    public object Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public bool WaitUntilLoaded
    {
        get => true;
    }

    protected object? Image { get; private set; }

    public abstract CompositionBrush GetAlphaMask();

    protected virtual void OnPlaceholderSourceChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    protected virtual Task<ImageSource?> ProvideCachedResourceAsync(Uri imageUri, CancellationToken token)
    {
        // By default we just use the built-in UWP image cache provided within the Image control.
        return Task.FromResult<ImageSource?>(new BitmapImage(imageUri));
    }

    protected virtual void OnImageOpened(object sender, RoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, LoadedState, true);
        ImageExOpened?.Invoke(this, new ImageExOpenedEventArgs());
    }

    protected virtual void OnImageFailed(object sender, ExceptionRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, FailedState, true);
        ImageExFailed?.Invoke(this, new ImageExFailedEventArgs(new FileNotFoundException(e.ErrorMessage)));
    }

    protected void AttachImageOpened(RoutedEventHandler handler)
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

    protected void RemoveImageOpened(RoutedEventHandler handler)
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

    protected void AttachImageFailed(ExceptionRoutedEventHandler handler)
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

    protected void RemoveImageFailed(ExceptionRoutedEventHandler handler)
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

    protected override void OnApplyTemplate()
    {
        RemoveImageOpened(OnImageOpened);
        RemoveImageFailed(OnImageFailed);

        Image = GetTemplateChild(PartImage);

        IsInitialized = true;

        ImageExInitialized?.Invoke(this, EventArgs.Empty);

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
    }

    private static void EnableLazyLoadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ImageExBase control)
        {
            bool value = (bool)e.NewValue;
            if (value)
            {
                control.LayoutUpdated += control.ImageExBase_LayoutUpdated;

                control.InvalidateLazyLoading();
            }
            else
            {
                control.LayoutUpdated -= control.ImageExBase_LayoutUpdated;
            }
        }
    }

    private static void LazyLoadingThresholdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ImageExBase control && control.EnableLazyLoading)
        {
            control.InvalidateLazyLoading();
        }
    }

    private static void PlaceholderSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ImageExBase control)
        {
            control.OnPlaceholderSourceChanged(e);
        }
    }

    private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ImageExBase control)
        {
            return;
        }

        if (e.OldValue is null || e.NewValue is null || !e.OldValue.Equals(e.NewValue))
        {
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
            ImageExOpened?.Invoke(this, new ImageExOpenedEventArgs());
        }
    }

    [SuppressMessage("", "IDE0019")]
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

        ImageSource? imageSource = source as ImageSource;
        if (imageSource is not null)
        {
            AttachSource(imageSource);

            return;
        }

        Uri? uri = source as Uri;
        if (uri is null)
        {
            string? url = source as string ?? source.ToString();
            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
            {
                VisualStateManager.GoToState(this, FailedState, true);
                ImageExFailed?.Invoke(this, new ImageExFailedEventArgs(new UriFormatException("Invalid uri specified.")));
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
        catch (OperationCanceledException)
        {
            // nothing to do as cancellation has been requested.
        }
        catch (Exception e)
        {
            VisualStateManager.GoToState(this, FailedState, true);
            ImageExFailed?.Invoke(this, new ImageExFailedEventArgs(e));
        }
    }

    private async Task LoadImageAsync(Uri imageUri, CancellationToken token)
    {
        if (imageUri is not null)
        {
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
    }

    private void ImageExBase_LayoutUpdated(object? sender, object e)
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

        Rect controlRect = TransformToVisual(hostElement)
            .TransformBounds(new Rect(0, 0, ActualWidth, ActualHeight));
        double lazyLoadingThreshold = LazyLoadingThreshold;
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