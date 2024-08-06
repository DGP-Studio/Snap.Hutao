// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint;

[Injection(InjectAs.Singleton, typeof(IHutaoEndpoints), Key = HutaoEndpointsKind.AlphaOS)]
internal sealed class HutaoEndpointsForAlphaOS : IHutaoEndpoints
{
    string IHomaRootAccess.Root { get => "https://homa.snapgenshin.com"; }

    string IInfrastructureRootAccess.Root { get => "https://api-alpha.snapgenshin.cn/global"; }
}