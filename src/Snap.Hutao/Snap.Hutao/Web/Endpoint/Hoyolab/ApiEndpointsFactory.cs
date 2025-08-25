// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hoyolab;

[ConstructorGenerated]
[Service(ServiceLifetime.Singleton, typeof(IApiEndpointsFactory))]
internal sealed partial class ApiEndpointsFactory : IApiEndpointsFactory
{
    private readonly IServiceProvider serviceProvider;

    public IApiEndpoints Create(ApiEndpointsKind kind)
    {
        return serviceProvider.GetRequiredKeyedService<IApiEndpoints>(kind);
    }
}