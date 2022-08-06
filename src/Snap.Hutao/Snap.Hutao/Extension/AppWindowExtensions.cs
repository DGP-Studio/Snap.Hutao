// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Windows.Graphics;

namespace Snap.Hutao.Extension;

/// <summary>
/// <see cref="AppWindow"/> 扩展
/// </summary>
public static class AppWindowExtensions
{
    /// <summary>
    /// 获取当前 <see cref="AppWindow"/> 的呈现矩形
    /// </summary>
    /// <param name="appWindow">当前 <see cref="AppWindow"/></param>
    /// <returns>呈现矩形</returns>
    public static RectInt32 GetRect(this AppWindow appWindow)
    {
        PointInt32 postion = appWindow.Position;
        SizeInt32 size = appWindow.Size;

        return new RectInt32(postion.X, postion.Y, size.Width, size.Height);
    }
}