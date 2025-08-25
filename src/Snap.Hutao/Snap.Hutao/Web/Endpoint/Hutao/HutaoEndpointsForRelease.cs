// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hutao;

[Service(ServiceLifetime.Singleton, typeof(IHutaoEndpoints), Key = HutaoEndpointsKind.Release)]
internal sealed class HutaoEndpointsForRelease : IHutaoEndpoints
{
    string IHomaRootAccess.Root { get => "https://homa.snapgenshin.com"; }

    string IInfrastructureRootAccess.Root { get => "https://api.snapgenshin.com"; }

    string IInfrastructureRawRootAccess.RawRoot { get => "https://api.snapgenshin.com"; }
}