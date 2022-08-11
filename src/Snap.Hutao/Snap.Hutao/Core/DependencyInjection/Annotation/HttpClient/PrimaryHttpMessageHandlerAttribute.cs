// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;

/// <summary>
/// 配置首选Http消息处理器特性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class PrimaryHttpMessageHandlerAttribute : Attribute
{
    /// <inheritdoc cref="System.Net.Http.HttpClientHandler.MaxConnectionsPerServer"/>
    public int MaxConnectionsPerServer { get; set; }
}