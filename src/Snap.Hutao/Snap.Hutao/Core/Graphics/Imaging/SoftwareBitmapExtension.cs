// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI;
using Snap.Hutao.Win32.System.WinRT;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using WinRT;

namespace Snap.Hutao.Core.Graphics.Imaging;

internal static class SoftwareBitmapExtension
{
    public static void NormalBlend(this SoftwareBitmap softwareBitmap, Bgra32 tint)
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

    public static Bgra32 GetBgra32AccentColor(this SoftwareBitmap softwareBitmap)
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

    public static Rgba32 GetRgba32AccentColor(this SoftwareBitmap softwareBitmap)
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

                return new((byte)(r / bytes.Length), (byte)(g / bytes.Length), (byte)(b / bytes.Length), (byte)(a / bytes.Length));
            }
        }
    }
}