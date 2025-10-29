// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.ViewModel.RoleCombat;
using Snap.Hutao.Web.Hutao.RoleCombat;
using System.Collections.Immutable;
using AvatarView = Snap.Hutao.ViewModel.Complex.AvatarView;

namespace Snap.Hutao.Service.Hutao;

[GeneratedConstructor]
[Service(ServiceLifetime.Singleton, typeof(IHutaoRoleCombatStatisticsCache))]
internal sealed partial class HutaoRoleCombatStatisticsCache : StatisticsCache, IHutaoRoleCombatStatisticsCache
{
    private readonly IServiceProvider serviceProvider;

    public int RecordTotal { get; private set; }

    public ImmutableArray<AvatarView> AvatarAppearances { get; private set; }

    public ValueTask InitializeForRoleCombatViewAsync(HutaoRoleCombatStatisticsMetadataContext context)
    {
        return InitializeForTypeAsync<RoleCombatViewModel, HutaoRoleCombatStatisticsMetadataContext>(context, AvatarAppearancesAsync);
    }

    [SuppressMessage("", "SH003")]
    private async Task AvatarAppearancesAsync(HutaoRoleCombatStatisticsMetadataContext context)
    {
        RoleCombatStatisticsItem raw;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoRoleCombatService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoRoleCombatService>();
            raw = await hutaoService.GetRoleCombatStatisticsItemAsync().ConfigureAwait(false);
        }

        RecordTotal = raw.RecordTotal;
        AvatarAppearances = [.. CurrentLeftJoinLast(raw.BackupAvatarRates.EmptyIfDefault().OrderByDescending(ir => ir.Rate), default, data => data.Item, (data, dataLast) => new AvatarView(context.GetAvatar(data.Item), data.Rate, dataLast?.Rate))];
    }
}