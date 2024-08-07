// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IHutaoEndpointsFactory))]
internal sealed partial class HutaoEndpointsFactory : IHutaoEndpointsFactory
{
    private readonly IServiceProvider serviceProvider;

    public IHutaoEndpoints Create()
    {
#if RELEASE
        return serviceProvider.GetRequiredKeyedService<IHutaoEndpoints>(HutaoEndpointsKind.Release);
#else
        return Core.Setting.LocalSetting.Get(Core.Setting.SettingKeys.AlphaBuildUseCNPatchEndpoint, false)
            ? serviceProvider.GetRequiredKeyedService<IHutaoEndpoints>(HutaoEndpointsKind.AlphaCN)
            : serviceProvider.GetRequiredKeyedService<IHutaoEndpoints>(HutaoEndpointsKind.AlphaOS);
#endif
    }
}