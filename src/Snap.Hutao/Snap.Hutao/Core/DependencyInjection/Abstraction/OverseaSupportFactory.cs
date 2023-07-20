// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Abstraction;

internal abstract class OverseaSupportFactory<TClient, TClientCN, TClientOS> : IOverseaSupportFactory<TClient>
    where TClientCN : notnull, TClient
    where TClientOS : notnull, TClient
{
    private readonly IServiceProvider serviceProvider;

    public OverseaSupportFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public TClient Create(bool isOversea)
    {
        return isOversea
            ? serviceProvider.GetRequiredService<TClientOS>()
            : serviceProvider.GetRequiredService<TClientCN>();
    }
}