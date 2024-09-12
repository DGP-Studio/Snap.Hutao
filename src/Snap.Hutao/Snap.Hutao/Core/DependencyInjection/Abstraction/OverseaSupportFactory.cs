// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Abstraction;

/// <summary>
/// 由于 AddHttpClient 不支持 KeyedService, 所以使用工厂模式
/// </summary>
/// <typeparam name="TClient">抽象类型</typeparam>
/// <typeparam name="TClientCN">官服/米游社类型</typeparam>
/// <typeparam name="TClientOS">国际/HoYoLAB类型</typeparam>
internal abstract class OverseaSupportFactory<TClient, TClientCN, TClientOS> : IOverseaSupportFactory<TClient>
    where TClientCN : notnull, TClient
    where TClientOS : notnull, TClient
{
    private readonly IServiceProvider serviceProvider;

    public OverseaSupportFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public virtual TClient Create(bool isOversea)
    {
        return isOversea
            ? serviceProvider.GetRequiredService<TClientOS>()
            : serviceProvider.GetRequiredService<TClientCN>();
    }
}