// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net.Http;

namespace Snap.Hutao;

[ConstructorGenerated(ResolveHttpClient = true)]
[Injection(InjectAs.Transient)]
internal sealed partial class SentryIpAddressConfigurator
{
    private readonly HttpClient httpClient;

    public async ValueTask ConfigureAsync()
    {
        string? ip = await httpClient.GetStringAsync("https://api.ipify.org/").ConfigureAwait(false);
        SentrySdk.ConfigureScope(scope => { scope.User.IpAddress = ip; });
    }
}