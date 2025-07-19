// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Metadata.Tower;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

internal sealed class BattleView
{
    private BattleView(TowerLevel towerLevel, uint battleIndex, SpiralAbyssMetadataContext context)
    {
        IndexValue = battleIndex;
        Gadget = battleIndex switch
        {
            1U => towerLevel.FirstGadget,
            2U => towerLevel.SecondGadget,
            _ => default,
        };
        MonsterWaves = battleIndex switch
        {
            1U => towerLevel.FirstWaves.SelectAsArray(static (w, context) => BattleWave.Create(w, context), context),
            2U => towerLevel.SecondWaves.SelectAsArray(static (w, context) => BattleWave.Create(w, context), context),
            _ => default!,
        };
    }

    public string? Time { get; private set; }

    public ImmutableArray<AvatarView> Avatars { get; private set; } = [];

    public NameDescription? Gadget { get; }

    public ImmutableArray<BattleWave> MonsterWaves { get; }

    internal uint IndexValue { get; }

    public static BattleView Create(TowerLevel level, uint index, SpiralAbyssMetadataContext context)
    {
        return new(level, index, context);
    }

    public void Attach(SpiralAbyssBattle battle, TimeSpan offset, SpiralAbyssMetadataContext context)
    {
        Time = $"{DateTimeOffset.FromUnixTimeSeconds(battle.Timestamp).ToOffset(offset):yyyy.MM.dd HH:mm:ss}";
        Avatars = battle.Avatars.SelectAsArray(static (a, context) => AvatarView.From(context.IdAvatarMap[a.Id]), context);
    }
}