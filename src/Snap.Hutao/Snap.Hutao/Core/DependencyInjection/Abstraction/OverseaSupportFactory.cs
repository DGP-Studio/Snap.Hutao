// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Abstraction;

[ConstructorGenerated]
internal abstract partial class OverseaSupportFactory<TClient, TClientCn, TClientOs> : IOverseaSupportFactory<TClient>
    where TClient : notnull
    where TClientCn : TClient
    where TClientOs : TClient
{
    private readonly IServiceProvider serviceProvider;

    public TClient Create(bool isOversea)
    {
        return isOversea
            ? serviceProvider.GetRequiredService<TClientOs>()
            : serviceProvider.GetRequiredService<TClientCn>();
    }
}