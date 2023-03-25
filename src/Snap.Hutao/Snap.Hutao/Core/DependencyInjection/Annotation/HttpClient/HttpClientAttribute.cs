// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;

/// <summary>
/// 指示被标注的类型可注入 HttpClient
/// 由源生成器生成注入代码
/// </summary>
[HighQuality]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
internal sealed class HttpClientAttribute : Attribute
{
    /// <summary>
    /// 构造一个新的特性
    /// </summary>
    /// <param name="configration">配置</param>
    public HttpClientAttribute(HttpClientConfiguration configration)
    {
    }
}
