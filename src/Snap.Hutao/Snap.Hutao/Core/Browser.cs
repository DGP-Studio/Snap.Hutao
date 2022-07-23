// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core;

/// <summary>
/// 封装了打开浏览器的方法
/// </summary>
public static class Browser
{
    /// <summary>
    /// 打开浏览器
    /// </summary>
    /// <param name="url">链接</param>
    /// <param name="failAction">失败时执行的回调</param>
    public static void Open(string url, Action<Exception>? failAction = null)
    {
        try
        {
            ProcessHelper.Start(url);
        }
        catch (Exception ex)
        {
            failAction?.Invoke(ex);
        }
    }
}
