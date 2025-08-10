// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;

internal enum HttpClientConfiguration
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
    /// Hoyolab app
    /// </summary>
    XRpc3,

    /// <summary>
    /// 米哈游启动器登录请求配置
    /// </summary>
    XRpc5,

    /// <summary>
    /// HoyoPlay 登录请求配置
    /// </summary>
    XRpc6,
}