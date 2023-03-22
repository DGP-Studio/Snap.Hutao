// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;

/// <summary>
/// Http客户端配置
/// </summary>
[HighQuality]
internal enum HttpClientConfigration
{
    /// <summary>
    /// 默认配置
    /// </summary>
    Default,

    /// <summary>
    /// 米游社请求配置
    /// </summary>
    XRpc,

    /// <summary>
    /// 米游社登录请求配置
    /// </summary>
    XRpc2,

    /// <summary>
    /// 国际服Hoyolab请求配置
    /// </summary>
    XRpc3,
}