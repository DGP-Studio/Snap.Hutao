// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.UI;
using Snap.Hutao.Win32.System.WinRT;
using System.Diagnostics;
using System.IO;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using WinRT;

namespace Snap.Hutao.Core.Caching;

internal static class MonoChromeImageConverter
{
    public static async ValueTask ConvertAndCopyToAsync(ElementTheme theme, Stream source, Stream destination)
    {
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);

        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(source.AsRandomAccessStream());

        // Always premultiplied to prevent some channels have a non-zero value when the alpha channel is zero
        using (SoftwareBitmap sourceBitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Premultiplied))
        {
            using (BitmapBuffer sourceBuffer = sourceBitmap.LockBuffer(BitmapBufferAccessMode.ReadWrite))
            {
                using (IMemoryBufferReference reference = sourceBuffer.CreateReference())
                {
                    byte value = (byte)(theme is ElementTheme.Light ? 0x00 : 0xFF);
                    Debug.Assert(Thread.CurrentThread.IsBackground);

                    reference.As<IMemoryBufferByteAccess>().GetBuffer(out Span<Rgba32> span);
                    foreach (ref Rgba32 pixel in span)
                    {
                        pixel.A = (byte)pixel.Luminance255;
                        pixel.R = pixel.G = pixel.B = value;
                    }
                }
            }

            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, destination.AsRandomAccessStream());
            encoder.SetSoftwareBitmap(sourceBitmap);
            await encoder.FlushAsync();
        }
    }
}