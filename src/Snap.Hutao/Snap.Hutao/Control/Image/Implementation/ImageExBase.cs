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

    public bool IsInitialized { get; private set; }

    public bool WaitUntilLoaded
    {
        get => true;
    }

    protected object? Image { get; private set; }

    protected object? PlaceholderImage { get; private set; }

    public abstract CompositionBrush GetAlphaMask();

    protected virtual Task<Uri?> ProvideCachedResourceAsync(Uri imageUri, CancellationToken token)
    {
        // By default we just use the built-in UWP image cache provided within the Image control.
        return Task.FromResult<Uri?>(imageUri);
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
        if (d is not ImageExBase control)
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
}