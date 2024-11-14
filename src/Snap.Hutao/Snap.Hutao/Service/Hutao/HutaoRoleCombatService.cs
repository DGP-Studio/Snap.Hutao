// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Hutao.RoleCombat;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.Hutao;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Scoped, typeof(IHutaoRoleCombatService))]
internal sealed partial class HutaoRoleCombatService : ObjectCacheService, IHutaoRoleCombatService
{
    public async ValueTask<RoleCombatStatisticsItem> GetRoleCombatStatisticsItemAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            HutaoRoleCombatClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoRoleCombatClient>();
            return await FromCacheOrWebAsync(nameof(RoleCombatStatisticsItem), last, homaClient.GetStatisticsAsync).ConfigureAwait(false);
        }
    }
}