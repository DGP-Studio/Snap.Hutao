// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net.Http;
using Snap.Hutao.Web.Endpoint.Hutao;

namespace Snap.Hutao.Core.LifeCycle;

[ConstructorGenerated(ResolveHttpClient = true)]
[Injection(InjectAs.Transient)]
internal sealed partial class SentryIpAddressConfigurator
{
    private readonly IHutaoEndpointsFactory hutaoEndpointsFactory;
    private readonly HttpClient httpClient;

    public async ValueTask ConfigureAsync()
    {
        string? ip = await httpClient.GetStringAsync(hutaoEndpointsFactory.Create().IpString()).ConfigureAwait(false);
        ip = ip.Trim('"');
        SentrySdk.ConfigureScope(scope => { scope.User.IpAddress = ip; });
    }
}