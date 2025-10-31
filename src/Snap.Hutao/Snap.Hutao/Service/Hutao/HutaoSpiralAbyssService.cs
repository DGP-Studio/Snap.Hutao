// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hutao.SpiralAbyss;

namespace Snap.Hutao.Service.Hutao;

[Service(ServiceLifetime.Scoped, typeof(IHutaoSpiralAbyssService))]
internal sealed partial class HutaoSpiralAbyssService : ObjectCacheService, IHutaoSpiralAbyssService
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial HutaoSpiralAbyssService(IServiceProvider serviceProvider);

    public override string TypeName { get; } = nameof(HutaoSpiralAbyssService);

    public async ValueTask<Overview> GetOverviewAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(Overview), last, homaClient.GetOverviewAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<IReadOnlyList<AvatarAppearanceRank>> GetAvatarAppearanceRanksAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(AvatarAppearanceRank), last, homaClient.GetAvatarAttendanceRatesAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<IReadOnlyList<AvatarUsageRank>> GetAvatarUsageRanksAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(AvatarUsageRank), last, homaClient.GetAvatarUtilizationRatesAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<IReadOnlyList<AvatarConstellationInfo>> GetAvatarConstellationInfosAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(AvatarConstellationInfo), last, homaClient.GetAvatarHoldingRatesAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<IReadOnlyList<AvatarCollocation>> GetAvatarCollocationsAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(AvatarCollocation), last, homaClient.GetAvatarCollocationsAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<IReadOnlyList<WeaponCollocation>> GetWeaponCollocationsAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(WeaponCollocation), last, homaClient.GetWeaponCollocationsAsync).ConfigureAwait(false);
        }
    }

    public async ValueTask<IReadOnlyList<TeamAppearance>> GetTeamAppearancesAsync(bool last = false)
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            HutaoSpiralAbyssClient homaClient = scope.ServiceProvider.GetRequiredService<HutaoSpiralAbyssClient>();
            return await FromCacheOrWebAsync(nameof(TeamAppearance), last, homaClient.GetTeamCombinationsAsync).ConfigureAwait(false);
        }
    }
}