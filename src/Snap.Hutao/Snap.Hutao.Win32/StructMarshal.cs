// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using Windows.Win32.UI.WindowsAndMessaging;

[assembly: InternalsVisibleTo("Snap.Hutao")]

namespace Snap.Hutao.Win32;

/// <summary>
/// 结构体封送
/// </summary>
internal static class StructMarshal
{
    /// <summary>
    /// 构造一个新的 <see cref="Windows.Win32.UI.WindowsAndMessaging.WINDOWPLACEMENT"/>
    /// </summary>
    /// <returns>新的实例</returns>
    public static unsafe WINDOWPLACEMENT WINDOWPLACEMENT()
    {
        return new() { length = unchecked((uint)sizeof(WINDOWPLACEMENT)) };
    }
}