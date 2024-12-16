// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hutao;

// 福建
[Injection(InjectAs.Singleton, typeof(IHutaoEndpoints), Key = HutaoEndpointsKind.AlphaCN)]
internal sealed class HutaoEndpointsForAlphaFJ : IHutaoEndpoints
{
    string IHomaRootAccess.Root { get => "https://homa.snapgenshin.com"; }

    string IInfrastructureRootAccess.Root { get => "https://alpha.snapgenshin.cn/fj"; }

    public string PatchSnapHutao()
    {
        return $"{((IInfrastructureRootAccess)this).Root}/patch/alpha";
    }
}