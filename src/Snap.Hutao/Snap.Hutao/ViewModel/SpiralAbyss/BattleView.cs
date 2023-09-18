// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Metadata.Tower;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

/// <summary>
/// 上下半视图
/// </summary>
[HighQuality]
internal sealed class BattleView : IMappingFrom<BattleView, TowerLevel, uint, SpiralAbyssMetadataContext>
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
            1U => towerLevel.FirstWaves.SelectList(w => BattleWave.From(w, context)),
            2U => towerLevel.SecondWaves.SelectList(w => BattleWave.From(w, context)),
            _ => default!,
        };
    }

    /// <summary>
    /// 时间
    /// </summary>
    public string? Time { get; private set; }

    /// <summary>
    /// 角色
    /// </summary>
    public List<AvatarView>? Avatars { get; private set; }

    public NameDescription? Gadget { get; }

    public List<BattleWave> MonsterWaves { get; }

    internal uint IndexValue { get; }

    public static BattleView From(TowerLevel level, uint index, SpiralAbyssMetadataContext context)
    {
        return new(level, index, context);
    }

    public void WithSpiralAbyssBattle(Battle battle, SpiralAbyssMetadataContext context)
    {
        Time = $"{DateTimeOffset.FromUnixTimeSeconds(battle.Timestamp).ToLocalTime():yyyy.MM.dd HH:mm:ss}";
        Avatars = battle.Avatars.SelectList(a => AvatarView.From(context.IdAvatarMap[a.Id]));
    }
}