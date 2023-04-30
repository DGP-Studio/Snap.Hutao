// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao;

/// <summary>
/// 应用程序资源提供器
/// </summary>
[Injection(InjectAs.Transient, typeof(IAppResourceProvider))]
internal sealed class AppResourceProvider : IAppResourceProvider
{
    private readonly App app;

    /// <summary>
    /// 构造一个新的应用程序资源提供器
    /// </summary>
    /// <param name="app">应用</param>
    public AppResourceProvider(App app)
    {
        this.app = app;
    }

    /// <inheritdoc/>
    public T GetResource<T>(string name)
    {
        return (T)app.Resources[name];
    }
}