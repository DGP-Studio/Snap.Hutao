// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using System.Diagnostics;

namespace Snap.Hutao.View.Extension;

/// <summary>
/// <see cref="CoreWebView2Environment"/> 扩展
/// </summary>
[HighQuality]
internal static class CoreWebView2EnvironmentExtension
{
    /// <summary>
    /// 退出
    /// </summary>
    /// <param name="environment">环境</param>
    public static void Exit(this CoreWebView2Environment environment)
    {
        // 暂不支持
        IReadOnlyList<CoreWebView2ProcessInfo> processInfos = environment.GetProcessInfos();

        foreach (CoreWebView2ProcessInfo processInfo in processInfos)
        {
            Process p = Process.GetProcessById(processInfo.ProcessId);
            if (p.ProcessName == "msedgewebview2.exe")
            {
                p.Kill();
            }
        }
    }
}
