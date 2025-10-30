// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Endpoint.Hutao;
using System.Net.Http;

namespace Snap.Hutao.Core.LifeCycle;

[Service(ServiceLifetime.Transient)]
internal sealed partial class SentryIpAddressEnricher
{
    private readonly IHutaoEndpointsFactory hutaoEndpointsFactory;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial SentryIpAddressEnricher(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask ConfigureAsync()
    {
        try
        {
            string ip = await httpClient.GetStringAsync(hutaoEndpointsFactory.Create().IpString()).ConfigureAwait(false);
            SentrySdk.ConfigureScope(static (scope, ip) => { scope.User.IpAddress = ip; }, ip.Trim('"'));
        }
        catch
        {
            // Man, what can I say?
        }
    }
}