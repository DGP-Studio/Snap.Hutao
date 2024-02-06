// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.System.WinRT;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using WinRT;

namespace Snap.Hutao.Control.Media;

/// <summary>
/// 软件位图拓展
/// </summary>
[HighQuality]
internal static class SoftwareBitmapExtension
{
    /// <summary>
    /// 混合模式 正常
    /// </summary>
    /// <param name="softwareBitmap">软件位图</param>
    /// <param name="tint">底色</param>
    public static unsafe void NormalBlend(this SoftwareBitmap softwareBitmap, Bgra32 tint)
    {
        using (BitmapBuffer buffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.ReadWrite))
        {
            using (IMemoryBufferReference reference = buffer.CreateReference())
            {
                reference.As<IMemoryBufferByteAccess>().GetBuffer(out Span<Bgra32> bytes);
                foreach (ref Bgra32 pixel in bytes)
                {
                    byte baseAlpha = pixel.A;
                    int opposite = 0xFF - baseAlpha;
                    pixel.B = (byte)(((pixel.B * baseAlpha) + (tint.B * opposite)) / 0xFF);
                    pixel.G = (byte)(((pixel.G * baseAlpha) + (tint.G * opposite)) / 0xFF);
                    pixel.R = (byte)(((pixel.R * baseAlpha) + (tint.R * opposite)) / 0xFF);
                    pixel.A = 0xFF;
                }
            }
        }
    }

    public static unsafe double Luminance(this SoftwareBitmap softwareBitmap)
    {
        using (BitmapBuffer buffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Read))
        {
            using (IMemoryBufferReference reference = buffer.CreateReference())
            {
                reference.As<IMemoryBufferByteAccess>().GetBuffer(out Span<Bgra32> bytes);
                double sum = 0;
                foreach (ref readonly Bgra32 pixel in bytes)
                {
                    sum += pixel.Luminance;
                }

                return sum / bytes.Length;
            }
        }
    }

    public static unsafe Bgra32 GetAccentColor(this SoftwareBitmap softwareBitmap)
    {
        using (BitmapBuffer buffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Read))
        {
            using (IMemoryBufferReference reference = buffer.CreateReference())
            {
                reference.As<IMemoryBufferByteAccess>().GetBuffer(out Span<Bgra32> bytes);
                double b = 0, g = 0, r = 0, a = 0;
                foreach (ref readonly Bgra32 pixel in bytes)
                {
                    b += pixel.B;
                    g += pixel.G;
                    r += pixel.R;
                    a += pixel.A;
                }

                return new((byte)(b / bytes.Length), (byte)(g / bytes.Length), (byte)(r / bytes.Length), (byte)(a / bytes.Length));
            }
        }
    }
}