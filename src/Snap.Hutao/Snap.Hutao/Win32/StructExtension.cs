// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Graphics;
using Windows.Win32.System.Diagnostics.ToolHelp;

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
}