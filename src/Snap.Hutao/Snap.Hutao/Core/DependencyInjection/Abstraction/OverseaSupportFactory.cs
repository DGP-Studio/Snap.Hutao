// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Abstraction;

[ConstructorGenerated]
internal abstract partial class OverseaSupportFactory<TClient, TClientCN, TClientOS> : IOverseaSupportFactory<TClient>
    where TClientCN : notnull, TClient
    where TClientOS : notnull, TClient
{
    private readonly IServiceProvider serviceProvider;

    public virtual TClient Create(bool isOversea)
    {
        return isOversea
            ? serviceProvider.GetRequiredService<TClientOS>()
            : serviceProvider.GetRequiredService<TClientCN>();
    }
}