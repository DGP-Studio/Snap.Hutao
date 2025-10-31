// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.ViewModel.Complex;
using Snap.Hutao.ViewModel.SpiralAbyss;
using Snap.Hutao.ViewModel.Wiki;
using Snap.Hutao.Web.Hutao.SpiralAbyss;
using System.Collections.Immutable;
using AvatarView = Snap.Hutao.ViewModel.Complex.AvatarView;

namespace Snap.Hutao.Service.Hutao;

[Service(ServiceLifetime.Singleton, typeof(IHutaoSpiralAbyssStatisticsCache))]
internal sealed partial class HutaoSpiralAbyssStatisticsCache : StatisticsCache, IHutaoSpiralAbyssStatisticsCache
{
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial HutaoSpiralAbyssStatisticsCache(IServiceProvider serviceProvider);

    public ImmutableArray<AvatarRankView> AvatarUsageRanks { get; set; }

    public ImmutableArray<AvatarRankView> AvatarAppearanceRanks { get; set; }

    public ImmutableArray<AvatarConstellationInfoView> AvatarConstellationInfos { get; set; }

    public ImmutableArray<TeamAppearanceView> TeamAppearances { get; set; }

    public Overview? Overview { get; set; }

    public ImmutableDictionary<AvatarId, AvatarCollocationView>? AvatarCollocations { get; set; }

    public ImmutableDictionary<WeaponId, WeaponCollocationView>? WeaponCollocations { get; set; }

    public ValueTask InitializeForSpiralAbyssViewAsync(HutaoSpiralAbyssStatisticsMetadataContext context)
    {
        return InitializeForTypeAsync<SpiralAbyssViewModel, HutaoSpiralAbyssStatisticsMetadataContext>(context, context =>
        {
            ReadOnlySpan<Task> tasks =
            [
                AvatarAppearanceRankAsync(context),
                AvatarUsageRanksAsync(context),
                AvatarConstellationInfosAsync(context),
                TeamAppearancesAsync(context),
                OverviewAsync(),
            ];

            return Task.WhenAll(tasks);
        });
    }

    public ValueTask InitializeForWikiAvatarViewAsync(HutaoSpiralAbyssStatisticsMetadataContext context)
    {
        return InitializeForTypeAsync<WikiAvatarViewModel, HutaoSpiralAbyssStatisticsMetadataContext>(context, AvatarCollocationsAsync);
    }

    public ValueTask InitializeForWikiWeaponViewAsync(HutaoSpiralAbyssStatisticsMetadataContext context)
    {
        return InitializeForTypeAsync<WikiWeaponViewModel, HutaoSpiralAbyssStatisticsMetadataContext>(context, WeaponCollocationsAsync);
    }

    [SuppressMessage("", "SH003")]
    private async Task AvatarCollocationsAsync(HutaoSpiralAbyssStatisticsMetadataContext context)
    {
        IReadOnlyList<AvatarCollocation> raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoSpiralAbyssService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoSpiralAbyssService>();
            raw = await hutaoService.GetAvatarCollocationsAsync(false).ConfigureAwait(false);
            rawLast = await hutaoService.GetAvatarCollocationsAsync(true).ConfigureAwait(false);
        }

        AvatarCollocations = CurrentLeftJoinLast(raw, rawLast, data => data.AvatarId, (raw, rawLast) => new AvatarCollocationView
        {
            AvatarId = raw.AvatarId,
            Avatars = [.. CurrentLeftJoinLast(raw.Avatars, rawLast?.Avatars, data => data.Item, (avatar, avatarLast) => new AvatarView(context.GetAvatar(avatar.Item), avatar.Rate, avatarLast?.Rate))],
            Weapons = [.. CurrentLeftJoinLast(raw.Weapons, rawLast?.Weapons, data => data.Item, (weapon, weaponLast) => new WeaponView(context.GetWeapon(weapon.Item), weapon.Rate, weaponLast?.Rate))],
            ReliquarySets = [.. CurrentLeftJoinLast(raw.Reliquaries, rawLast?.Reliquaries, data => data.Item, (relic, relicLast) => new ReliquarySetView(context.ExtendedIdReliquarySetMap, relic, relicLast))],
        }).ToImmutableDictionary(a => a.AvatarId);
    }

    [SuppressMessage("", "SH003")]
    private async Task WeaponCollocationsAsync(HutaoSpiralAbyssStatisticsMetadataContext context)
    {
        IReadOnlyList<WeaponCollocation> raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoSpiralAbyssService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoSpiralAbyssService>();
            raw = await hutaoService.GetWeaponCollocationsAsync(false).ConfigureAwait(false);
            rawLast = await hutaoService.GetWeaponCollocationsAsync(true).ConfigureAwait(false);
        }

        WeaponCollocations = CurrentLeftJoinLast(raw, rawLast, data => data.WeaponId, (raw, rawLast) => new WeaponCollocationView
        {
            WeaponId = raw.WeaponId,
            Avatars = [.. CurrentLeftJoinLast(raw.Avatars, rawLast?.Avatars, data => data.Item, (avatar, avatarLast) => new AvatarView(context.GetAvatar(avatar.Item), avatar.Rate, avatarLast?.Rate))],
        }).ToImmutableDictionary(w => w.WeaponId);
    }

    [SuppressMessage("", "SH003")]
    private async Task AvatarAppearanceRankAsync(HutaoSpiralAbyssStatisticsMetadataContext context)
    {
        IReadOnlyList<AvatarAppearanceRank> raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoSpiralAbyssService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoSpiralAbyssService>();
            raw = await hutaoService.GetAvatarAppearanceRanksAsync(false).ConfigureAwait(false);
            rawLast = await hutaoService.GetAvatarAppearanceRanksAsync(true).ConfigureAwait(false);
        }

        AvatarAppearanceRanks =
        [
            .. CurrentLeftJoinLast(raw.OrderByDescending(r => r.Floor), rawLast, data => data.Floor, (raw, rawLast) => new AvatarRankView
            {
                Floor = SH.FormatModelBindingHutaoComplexRankFloor(raw.Floor),
                Avatars = [..CurrentLeftJoinLast(raw.Ranks.SortByDescending(r => r.Rate), rawLast?.Ranks, data => data.Item, (rank, rankLast) => new AvatarView(context.GetAvatar(rank.Item), rank.Rate, rankLast?.Rate))],
            })
        ];
    }

    [SuppressMessage("", "SH003")]
    private async Task AvatarUsageRanksAsync(HutaoSpiralAbyssStatisticsMetadataContext context)
    {
        IReadOnlyList<AvatarUsageRank> raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoSpiralAbyssService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoSpiralAbyssService>();
            raw = await hutaoService.GetAvatarUsageRanksAsync(false).ConfigureAwait(false);
            rawLast = await hutaoService.GetAvatarUsageRanksAsync(true).ConfigureAwait(false);
        }

        AvatarUsageRanks =
        [
            .. CurrentLeftJoinLast(raw.OrderByDescending(r => r.Floor), rawLast, data => data.Floor, (raw, rawLast) => new AvatarRankView
            {
                Floor = SH.FormatModelBindingHutaoComplexRankFloor(raw.Floor),
                Avatars = [.. CurrentLeftJoinLast(raw.Ranks.SortByDescending(r => r.Rate), rawLast?.Ranks, data => data.Item, (rank, rankLast) => new AvatarView(context.GetAvatar(rank.Item), rank.Rate, rankLast?.Rate))],
            })
        ];
    }

    [SuppressMessage("", "SH003")]
    private async Task AvatarConstellationInfosAsync(HutaoSpiralAbyssStatisticsMetadataContext context)
    {
        IReadOnlyList<AvatarConstellationInfo> raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoSpiralAbyssService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoSpiralAbyssService>();
            raw = await hutaoService.GetAvatarConstellationInfosAsync(false).ConfigureAwait(false);
            rawLast = await hutaoService.GetAvatarConstellationInfosAsync(true).ConfigureAwait(false);
        }

        AvatarConstellationInfos =
        [
            .. CurrentLeftJoinLast(raw.OrderBy(i => i.HoldingRate), rawLast, data => data.AvatarId, (raw, rawLast) => new AvatarConstellationInfoView(context.GetAvatar(raw.AvatarId), raw.HoldingRate, rawLast?.HoldingRate)
            {
                Rates = [.. CurrentLeftJoinLast(raw.Constellations, rawLast?.Constellations, data => data.Item, (rate, rateLast) => new RateAndDelta(rate.Rate, rateLast?.Rate))],
            })
        ];
    }

    [SuppressMessage("", "SH003")]
    private async Task TeamAppearancesAsync(HutaoSpiralAbyssStatisticsMetadataContext context)
    {
        IReadOnlyList<TeamAppearance> teamAppearancesRaw;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoSpiralAbyssService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoSpiralAbyssService>();
            teamAppearancesRaw = await hutaoService.GetTeamAppearancesAsync().ConfigureAwait(false);
        }

        TeamAppearances = [.. teamAppearancesRaw.OrderByDescending(t => t.Floor).Select(team => new TeamAppearanceView(team, context.IdAvatarMap))];
    }

    [SuppressMessage("", "SH003")]
    private async Task OverviewAsync()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoSpiralAbyssService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoSpiralAbyssService>();
            Overview = await hutaoService.GetOverviewAsync().ConfigureAwait(false);
        }
    }
}