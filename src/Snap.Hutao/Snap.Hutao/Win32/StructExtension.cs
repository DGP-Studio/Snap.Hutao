// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Graphics;
using Windows.Win32.Foundation;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Win32;

/// <summary>
/// 结构扩展
/// </summary>
internal static class StructExtension
{
    /// <summary>
    /// 比例缩放
    /// </summary>
    /// <param name="rectInt32">源</param>
    /// <param name="scale">比例</param>
    /// <returns>结果</returns>
    public static RectInt32 Scale(this RectInt32 rectInt32, double scale)
    {
        return new((int)(rectInt32.X * scale), (int)(rectInt32.Y * scale), (int)(rectInt32.Width * scale), (int)(rectInt32.Height * scale));
    }

    /// <summary>
    /// 尺寸
    /// </summary>
    /// <param name="rectInt32">源</param>
    /// <returns>结果</returns>
    public static int Size(this RectInt32 rectInt32)
    {
        return rectInt32.Width * rectInt32.Height;
    }

    /// <summary>
    /// 尺寸
    /// </summary>
    /// <param name="sizeInt32">源</param>
    /// <returns>结果</returns>
    public static int Size(this SizeInt32 sizeInt32)
    {
        return sizeInt32.Width * sizeInt32.Height;
    }

    /// <summary>
    /// 使用完成后自动关闭句柄
    /// </summary>
    /// <param name="handle">句柄</param>
    /// <returns>用于关闭句柄的对象</returns>
    public static IDisposable AutoClose(this HANDLE handle)
    {
        return new HandleCloser(handle);
    }

    private readonly struct HandleCloser : IDisposable
    {
        private readonly HANDLE handle;

        public HandleCloser(HANDLE handle)
        {
            this.handle = handle;
        }

        public void Dispose()
        {
            CloseHandle(handle);
        }
    }
}