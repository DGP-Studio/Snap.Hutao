// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Win32;
using Windows.Win32.System.WinRT;
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
    public static unsafe void NormalBlend(this SoftwareBitmap softwareBitmap, Bgra8 tint)
    {
        using (BitmapBuffer buffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.ReadWrite))
        {
            using (IMemoryBufferReference reference = buffer.CreateReference())
            {
                reference.As<IMemoryBufferByteAccess>().GetBuffer(out byte* data, out uint length);

                for (int i = 0; i < length; i += 4)
                {
                    Bgra8* pixel = (Bgra8*)(data + i);
                    byte baseAlpha = pixel->A;
                    pixel->B = (byte)(((pixel->B * baseAlpha) + (tint.B * (0xFF - baseAlpha))) / 0xFF);
                    pixel->G = (byte)(((pixel->G * baseAlpha) + (tint.G * (0xFF - baseAlpha))) / 0xFF);
                    pixel->R = (byte)(((pixel->R * baseAlpha) + (tint.R * (0xFF - baseAlpha))) / 0xFF);
                    pixel->A = 0xFF;
                }
            }
        }
    }
}