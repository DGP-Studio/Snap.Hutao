// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

// Comment or Uncomment to switch endpoint behavior
#define FORCE_USE_RELEASE_ENDPOINT

namespace Snap.Hutao.Web.Endpoint.Hutao;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IHutaoEndpointsFactory))]
internal sealed partial class HutaoEndpointsFactory : IHutaoEndpointsFactory
{
    private readonly IServiceProvider serviceProvider;

    public IHutaoEndpoints Create()
    {
#if RELEASE && !IS_ALPHA_BUILD
        return serviceProvider.GetRequiredKeyedService<IHutaoEndpoints>(HutaoEndpointsKind.Release);
#elif DEBUG && FORCE_USE_RELEASE_ENDPOINT
        return serviceProvider.GetRequiredKeyedService<IHutaoEndpoints>(HutaoEndpointsKind.Release);
#else
        return Core.Setting.LocalSetting.Get(Core.Setting.SettingKeys.AlphaBuildUseCNPatchEndpoint, false)
            ? serviceProvider.GetRequiredKeyedService<IHutaoEndpoints>(HutaoEndpointsKind.AlphaCN)
            : serviceProvider.GetRequiredKeyedService<IHutaoEndpoints>(HutaoEndpointsKind.AlphaOS);
#endif
    }
}