// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.DataTransfer;
using Snap.Hutao.Core.ExceptionService;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Graphics.Imaging;
using Windows.Media.Casting;
using Windows.Storage.Streams;

namespace Snap.Hutao.UI.Xaml.Control.Image;

[SuppressMessage("", "CA1001")]
[SuppressMessage("", "SH003")]
[TemplateVisualState(Name = LoadingState, GroupName = CommonGroup)]
[TemplateVisualState(Name = LoadedState, GroupName = CommonGroup)]
[TemplateVisualState(Name = UnloadedState, GroupName = CommonGroup)]
[TemplateVisualState(Name = FailedState, GroupName = CommonGroup)]
[TemplatePart(Name = PartImage, Type = typeof(object))]
[TemplatePart(Name = PartPlaceholderImage, Type = typeof(object))]
[DependencyProperty("SourceName", typeof(string), "Unknown")]
[DependencyProperty("CachedName", typeof(string), "Unknown")]
[DependencyProperty("NineGrid", typeof(Thickness))]
[DependencyProperty("Stretch", typeof(Stretch), Stretch.Uniform)]
[DependencyProperty("PlaceholderSource", typeof(object), default(object))]
[DependencyProperty("PlaceholderStretch", typeof(Stretch), Stretch.Uniform)]
[DependencyProperty("PlaceholderMargin", typeof(Thickness))]
[DependencyProperty("Source", typeof(object), default(object), nameof(SourceChanged))]
internal sealed partial class CachedImage : Microsoft.UI.Xaml.Controls.Control, IAlphaMaskProvider
{
    private const string PartImage = "Image";
    private const string PartPlaceholderImage = "PlaceholderImage";
    private const string CommonGroup = "CommonStates";
    private const string LoadingState = "Loading";
    private const string LoadedState = "Loaded";
    private const string UnloadedState = "Unloaded";
    private const string FailedState = "Failed";

    private CancellationTokenSource? tokenSource;

    public CachedImage()
    {
        DefaultStyleKey = typeof(CachedImage);
    }

    public bool IsInitialized { get; private set; }

    public bool WaitUntilLoaded
    {
        get => true;
    }

    private object? Image { get; set; }

    private object? PlaceholderImage { get; set; }

    public CompositionBrush GetAlphaMask()
    {
        if (IsInitialized && Image is Microsoft.UI.Xaml.Controls.Image image)
        {
            return image.GetAlphaMask();
        }

        return default!;
    }

    public CastingSource GetAsCastingSource()
    {
        if (IsInitialized && Image is Microsoft.UI.Xaml.Controls.Image image)
        {
            return image.GetAsCastingSource();
        }

        return default!;
    }

    protected override void OnApplyTemplate()
    {
        RemoveImageOpened(OnImageOpened);
        RemoveImageFailed(OnImageFailed);

        Image = GetTemplateChild(PartImage);

        IsInitialized = true;

        SetSource(Source);

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

    private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not CachedImage control)
        {
            return;
        }

        if (e.OldValue is not null && e.NewValue is not null && e.OldValue.Equals(e.NewValue))
        {
            return;
        }

        control.SetSource(e.NewValue);
    }

    private static bool IsHttpUri(Uri uri)
    {
        return uri.IsAbsoluteUri && (uri.Scheme == "http" || uri.Scheme == "https");
    }

    private async Task<Uri?> ProvideCachedResourceAsync(Uri imageUri, CancellationToken token)
    {
        SourceName = Path.GetFileName(imageUri.ToString());
        IImageCache imageCache = this.ServiceProvider().GetRequiredService<IImageCache>();

        try
        {
            HutaoException.ThrowIf(string.IsNullOrEmpty(imageUri.Host), SH.ControlImageCachedImageInvalidResourceUri);
            string file = await imageCache.GetFileFromCacheAsync(imageUri).ConfigureAwait(true); // BitmapImage need to be created by main thread.
            CachedName = Path.GetFileName(file);
            token.ThrowIfCancellationRequested(); // check token state to determine whether the operation should be canceled.
            return file.ToUri();
        }
        catch (COMException)
        {
            // The image is corrupted, remove it.
            imageCache.Remove(imageUri);
            return default;
        }
    }

    private void OnImageOpened(object sender, RoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, LoadedState, true);
    }

    private void OnImageFailed(object sender, ExceptionRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, FailedState, true);
    }

    private void AttachSource(BitmapImage? source, Uri? uri)
    {
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
        else
        {
            // https://learn.microsoft.com/en-us/windows/uwp/debug-test-perf/optimize-animations-and-media#optimize-image-resources
            source.UriSource = uri;
            VisualStateManager.GoToState(this, LoadedState, true);
        }
    }

    private void AttachPlaceholderSource(BitmapImage? source, Uri? uri)
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
        else
        {
            // https://learn.microsoft.com/en-us/windows/uwp/debug-test-perf/optimize-animations-and-media#optimize-image-resources
            source.UriSource = uri;
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

        AttachSource(default, default);

        if (source is null)
        {
            return;
        }

        VisualStateManager.GoToState(this, LoadingState, true);

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

        AttachPlaceholderSource(default, default);

        if (source is null)
        {
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

            Uri? actualUri = await ProvideCachedResourceAsync(uri, tokenSource.Token).ConfigureAwait(true);

            ArgumentNullException.ThrowIfNull(tokenSource);
            if (!tokenSource.IsCancellationRequested)
            {
                // Only attach our image if we still have a valid request.
                AttachPlaceholderSource(new BitmapImage(), actualUri);
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

        Uri? actualUri = await ProvideCachedResourceAsync(imageUri, token).ConfigureAwait(true);

        ArgumentNullException.ThrowIfNull(tokenSource);
        if (!tokenSource.IsCancellationRequested)
        {
            // Only attach our image if we still have a valid request.
            AttachSource(new BitmapImage(), actualUri);
        }
    }

    [Command("CopyToClipboardCommand")]
    private async Task CopyToClipboard()
    {
        if (Image is Microsoft.UI.Xaml.Controls.Image { Source: BitmapImage bitmap })
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
                        Ioc.Default.GetRequiredService<IClipboardProvider>().SetBitmap(memory);
                    }
                }
            }
        }
    }
}