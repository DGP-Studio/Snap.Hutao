// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Core;

/// <summary>
/// 进程帮助类
/// </summary>
public static class ProcessHelper
{
    /// <summary>
    /// 启动进程
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="useShellExecute">使用shell</param>
    /// <returns>进程</returns>
    public static Process? Start(string path, bool useShellExecute = true)
    {
        ProcessStartInfo processInfo = new(path)
        {
            UseShellExecute = useShellExecute,
        };
        return Process.Start(processInfo);
    }

    /// <summary>
    /// 启动进程
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="arguments">命令行参数</param>
    /// <param name="useShellExecute">使用shell</param>
    /// <returns>进程</returns>
    public static Process? Start(string path, string arguments, bool useShellExecute = true)
    {
        ProcessStartInfo processInfo = new(path)
        {
            UseShellExecute = useShellExecute,
            Arguments = arguments,
        };
        return Process.Start(processInfo);
    }
}
