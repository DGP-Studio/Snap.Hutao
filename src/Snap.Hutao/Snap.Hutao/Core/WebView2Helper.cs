// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;

namespace Snap.Hutao.Core;

/// <summary>
/// 检测 WebView2运行时 是否存在
/// 不再使用注册表检查方式
/// </summary>
internal class WebView2Helper
{
    private static bool hasEverDetected = false;
    private static bool isSupported = false;

    /// <summary>
    /// 检测 WebView2 是否存在
    /// </summary>
    public static bool IsSupported
    {
        get
        {
            if (hasEverDetected)
            {
                return isSupported;
            }
            else
            {
                hasEverDetected = true;
                isSupported = true;
                try
                {
                    string version = CoreWebView2Environment.GetAvailableBrowserVersionString();
                }
                catch (Exception ex)
                {
                    Ioc.Default.GetRequiredService<ILogger<WebView2Helper>>().LogError(ex, "WebView2 运行时未安装");
                    isSupported = false;
                }

                return isSupported;
            }
        }
    }
}
