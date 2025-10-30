// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hoyolab;

[Service(ServiceLifetime.Singleton, typeof(IApiEndpointsFactory))]
internal sealed partial class ApiEndpointsFactory : IApiEndpointsFactory
{
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial ApiEndpointsFactory(IServiceProvider serviceProvider);

    public IApiEndpoints Create(ApiEndpointsKind kind)
    {
        return serviceProvider.GetRequiredKeyedService<IApiEndpoints>(kind);
    }
}