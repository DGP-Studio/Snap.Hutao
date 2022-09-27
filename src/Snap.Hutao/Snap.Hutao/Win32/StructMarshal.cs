// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Graphics;
using Windows.Win32.System.Diagnostics.ToolHelp;

namespace Snap.Hutao.Win32;

/// <summary>
/// 结构体封送
/// </summary>
internal static class StructMarshal
{
    /// <summary>
    /// 构造一个新的 <see cref="Windows.Win32.System.Diagnostics.ToolHelp.MODULEENTRY32"/>
    /// </summary>
    /// <returns>新的实例</returns>
    public static unsafe MODULEENTRY32 MODULEENTRY32()
    {
        return new() { dwSize = (uint)sizeof(MODULEENTRY32) };
    }

    /// <summary>
    /// 构造一个新的<see cref="Windows.Graphics.RectInt32"/>
    /// </summary>
    /// <param name="size">尺寸</param>
    /// <returns>新的实例</returns>
    public static RectInt32 RectInt32(SizeInt32 size)
    {
        return new(0, 0, size.Width, size.Height);
    }

    /// <summary>
    /// 判断结构实例是否为默认结构
    /// </summary>
    /// <param name="moduleEntry32">待测试的结构</param>
    /// <returns>是否为默认结构</returns>
    public static bool IsDefault(MODULEENTRY32 moduleEntry32)
    {
        return moduleEntry32.dwSize == 0;
    }
}