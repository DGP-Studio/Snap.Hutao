// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Hutao;

namespace Snap.Hutao.ViewModel.Complex;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class HutaoRoleCombatDatabaseViewModel : Abstraction.ViewModel
{
    private readonly IHutaoRoleCombatStatisticsCache hutaoCache;
    private readonly ITaskContext taskContext;

    private int recordTotal;
    private List<AvatarView>? avatarAppearances;

    public int RecordTotal { get => recordTotal; set => SetProperty(ref recordTotal, value); }

    public List<AvatarView>? AvatarAppearances { get => avatarAppearances; set => SetProperty(ref avatarAppearances, value); }

    /// <inheritdoc/>
    protected override async Task LoadAsync()
    {
        if (await hutaoCache.InitializeForRoleCombatViewAsync().ConfigureAwait(false))
        {
            await taskContext.SwitchToMainThreadAsync();

            RecordTotal = hutaoCache.RecordTotal;
            AvatarAppearances = hutaoCache.AvatarAppearances;
        }
    }
}
