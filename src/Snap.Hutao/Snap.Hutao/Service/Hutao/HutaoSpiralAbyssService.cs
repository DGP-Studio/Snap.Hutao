// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hutao.SpiralAbyss;

namespace Snap.Hutao.Service.Hutao;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Scoped, typeof(IHutaoSpiralAbyssService))]
internal sealed partial class HutaoSpiralAbyssService : ObjectCacheService, IHutaoSpiralAbyssService
{
    public async ValueTask<Overview> GetOverviewAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(Overview), last, homaClient.GetOverviewAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<List<AvatarAppearanceRank>> GetAvatarAppearanceRanksAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(AvatarAppearanceRank), last, homaClient.GetAvatarAttendanceRatesAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<List<AvatarUsageRank>> GetAvatarUsageRanksAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(AvatarUsageRank), last, homaClient.GetAvatarUtilizationRatesAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<List<AvatarConstellationInfo>> GetAvatarConstellationInfosAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(AvatarConstellationInfo), last, homaClient.GetAvatarHoldingRatesAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<List<AvatarCollocation>> GetAvatarCollocationsAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(AvatarCollocation), last, homaClient.GetAvatarCollocationsAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<List<WeaponCollocation>> GetWeaponCollocationsAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(WeaponCollocation), last, homaClient.GetWeaponCollocationsAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<List<TeamAppearance>> GetTeamAppearancesAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(TeamAppearance), last, homaClient.GetTeamCombinationsAsync).ConfigureAwait(false);
        }
    }
}