// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hutao.RoleCombat;

namespace Snap.Hutao.Service.Hutao;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Scoped, typeof(IHutaoRoleCombatService))]
internal sealed partial class HutaoRoleCombatService : ObjectCacheService, IHutaoRoleCombatService
{
    public async ValueTask<RoleCombatStatisticsItem> GetRoleCombatStatisticsItemAsync()
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            HutaoRoleCombatClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoRoleCombatClient>();
            return await FromCacheOrWebAsync(nameof(RoleCombatStatisticsItem), false, homaClient.GetStatisticsAsync).ConfigureAwait(false);
        }
    }
}