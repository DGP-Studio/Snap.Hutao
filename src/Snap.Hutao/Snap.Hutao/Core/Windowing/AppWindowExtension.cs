// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Windows.Graphics;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// <see cref="AppWindow"/> 扩展
/// </summary>
[HighQuality]
internal static class AppWindowExtension
{
    /// <summary>
    /// 获取当前 <see cref="AppWindow"/> 的呈现矩形
    /// </summary>
    /// <param name="appWindow">当前 <see cref="AppWindow"/></param>
    /// <returns>呈现矩形</returns>
    public static RectInt32 GetRect(this AppWindow appWindow)
    {
        return StructMarshal.RectInt32(appWindow.Position, appWindow.Size);
    }
}