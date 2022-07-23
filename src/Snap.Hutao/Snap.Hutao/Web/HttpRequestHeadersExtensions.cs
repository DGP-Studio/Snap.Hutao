// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net.Http.Headers;

namespace Snap.Hutao.Web;

/// <summary>
/// <see cref="HttpRequestHeaders"/> 扩展
/// </summary>
public static class HttpRequestHeadersExtensions
{
    /// <summary>
    /// 设置请求头的值
    /// </summary>
    /// <param name="headers">请求头</param>
    /// <param name="name">名称</param>
    /// <param name="value">值</param>
    public static void Set(this HttpRequestHeaders headers, string name, string? value)
    {
        headers.Clear();
        headers.Add(name, value);
    }
}
