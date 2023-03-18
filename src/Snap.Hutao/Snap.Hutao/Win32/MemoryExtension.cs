// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.Foundation;

namespace Snap.Hutao.Win32;

/// <summary>
/// 内存拓展 for <see cref="Memory{T}"/> and <see cref="Span{T}"/>
/// </summary>
[HighQuality]
internal static class MemoryExtension
{
    /// <summary>
    /// 将 __CHAR_256 转换到 字符串
    /// </summary>
    /// <param name="char256">目标字符数组</param>
    /// <returns>结果字符串</returns>
    public static unsafe ReadOnlySpan<byte> AsNullTerminatedReadOnlySpan(this in __CHAR_256 char256)
    {
        fixed (CHAR* pszChar = &char256._0)
        {
            return MemoryMarshal.CreateReadOnlySpanFromNullTerminated((byte*)pszChar);
        }
    }
}