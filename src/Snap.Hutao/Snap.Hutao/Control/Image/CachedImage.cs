// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Core.Caching;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO.DataTransfer;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace Snap.Hutao.Control.Image;

/// <summary>
/// 缓存图像
/// </summary>
[HighQuality]
[DependencyProperty("SourceName", typeof(string), "Unknown")]
[DependencyProperty("CachedName", typeof(string), "Unknown")]
internal sealed partial class CachedImage : Implementation.ImageEx
{
    private string? file;

    /// <summary>
    /// 构造一个新的缓存图像
    /// </summary>
    public CachedImage()
    {
        DefaultStyleKey = typeof(CachedImage);
        DefaultStyleResourceUri = "ms-appx:///Control/Image/CachedImage.xaml".ToUri();
    }

    /// <inheritdoc/>
    protected override async Task<Uri?> ProvideCachedResourceAsync(Uri imageUri, CancellationToken token)
    {
        SourceName = Path.GetFileName(imageUri.ToString());
        IImageCache imageCache = this.ServiceProvider().GetRequiredService<IImageCache>();

        try
        {
            HutaoException.ThrowIf(string.IsNullOrEmpty(imageUri.Host), SH.ControlImageCachedImageInvalidResourceUri);
            string file = await imageCache.GetFileFromCacheAsync(imageUri).ConfigureAwait(true); // BitmapImage need to be created by main thread.
            CachedName = Path.GetFileName(file);
            this.file = file;
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