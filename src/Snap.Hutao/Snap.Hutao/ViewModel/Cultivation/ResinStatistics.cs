// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Cultivation;

internal sealed class ResinStatistics
{
    public ImmutableArray<WorldDropProability> WorldDropProabilities { get; } = WorldDropProability.WorldDropProabilities;

    public WorldDropProability SelectedWorldDropProability
    {
        get;
        set
        {
            field = value;
            BlossomOfWealth.SelectedWorldDropProability = value;
            BlossomOfRevelation.SelectedWorldDropProability = value;
            TalentBooks.SelectedWorldDropProability = value;
            WeaponAscension.SelectedWorldDropProability = value;
            NormalBoss.SelectedWorldDropProability = value;
            WeeklyBoss.SelectedWorldDropProability = value;
        }
    } = WorldDropProability.Nine;

    public ResinStatisticsItem BlossomOfWealth { get; } = new("藏金之花", ResinStatisticsItemKind.BlossomOfWealth, 20, true);

    public ResinStatisticsItem BlossomOfRevelation { get; } = new("启示之花", ResinStatisticsItemKind.BlossomOfRevelation, 20, true);

    public ResinStatisticsItem TalentBooks { get; } = new("角色天赋素材", ResinStatisticsItemKind.TalentBooks, 20, true);

    public ResinStatisticsItem WeaponAscension { get; } = new("武器突破素材", ResinStatisticsItemKind.WeaponAscension, 20, true);

    public ResinStatisticsItem NormalBoss { get; } = new("世界 BOSS", ResinStatisticsItemKind.NormalBoss, 40, false);

    public ResinStatisticsItem WeeklyBoss { get; } = new("周常 BOSS", ResinStatisticsItemKind.WeeklyBoss, 60, false);
}