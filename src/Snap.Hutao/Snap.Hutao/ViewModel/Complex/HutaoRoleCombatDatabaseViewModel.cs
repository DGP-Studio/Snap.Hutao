// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Complex;

[ConstructorGenerated]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class HutaoRoleCombatDatabaseViewModel : Abstraction.ViewModel
{
    private readonly IHutaoRoleCombatStatisticsCache hutaoCache;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    public int RecordTotal { get; set => SetProperty(ref field, value); }

    public ImmutableArray<AvatarView> AvatarAppearances { get; set => SetProperty(ref field, value); }

    protected override async Task LoadAsync()
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return;
        }

        HutaoRoleCombatStatisticsMetadataContext context = await metadataService.GetContextAsync<HutaoRoleCombatStatisticsMetadataContext>().ConfigureAwait(false);
        await hutaoCache.InitializeForRoleCombatViewAsync(context).ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();

        RecordTotal = hutaoCache.RecordTotal;
        AvatarAppearances = hutaoCache.AvatarAppearances;
    }
}
