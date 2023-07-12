// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;

/// <summary>
/// 指示被标注的类型可注入 HttpClient
/// 由源生成器生成注入代码
/// </summary>
[HighQuality]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal sealed class HttpClientAttribute : Attribute
{
    /// <summary>
    /// 构造一个新的特性
    /// </summary>
    /// <param name="configuration">配置</param>
    public HttpClientAttribute(HttpClientConfiguration configuration)
    {
    }

    /// <summary>
    /// 构造一个新的特性
    /// </summary>
    /// <param name="configuration">配置</param>
    /// <param name="interfaceType">实现的接口类型</param>
    public HttpClientAttribute(HttpClientConfiguration configuration, Type interfaceType)
    {
    }
}
