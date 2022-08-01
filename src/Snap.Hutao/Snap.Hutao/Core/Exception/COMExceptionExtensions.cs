// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.Exception;

/// <summary>
/// COM异常扩展
/// </summary>
internal static class COMExceptionExtensions
{
    /// <summary>
    /// 比较COM异常是否与某个错误代码等价
    /// </summary>
    /// <param name="exception">异常</param>
    /// <param name="code">错误代码</param>
    /// <returns>是否为该错误</returns>
    public static bool Is(this COMException exception, COMError code)
    {
        return exception.HResult == unchecked((int)code);
    }
}