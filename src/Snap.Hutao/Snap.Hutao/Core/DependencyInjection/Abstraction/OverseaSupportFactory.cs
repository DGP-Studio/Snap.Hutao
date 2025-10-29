// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Abstraction;

internal abstract partial class OverseaSupportFactory<TClient, TClientCN, TClientOS> : IOverseaSupportFactory<TClient>
    where TClient : notnull
    where TClientCN : TClient
    where TClientOS : TClient
{
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial OverseaSupportFactory(IServiceProvider serviceProvider);

    public TClient Create(bool isOversea)
    {
        return isOversea
            ? serviceProvider.GetRequiredService<TClientOS>()
            : serviceProvider.GetRequiredService<TClientCN>();
    }
}