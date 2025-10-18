// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.RoleCombat;

internal sealed class RoundView
{
    private RoundView(RoleCombatRoundData data, in TimeSpan offset, RoleCombatMetadataContext context)
    {
        FinishTime = DateTimeOffset.FromUnixTimeSeconds(data.FinishTime).ToOffset(offset);

        RoundId = data.IsTarot
            ? SH.FormatViewModelRoleCombatTarot(data.TarotSerialNumber.ToRoman())
            : SH.FormatViewModelRoleCombatRound(data.RoundId);
        IsGetMedal = data.IsGetMedal;
        IsTarot = data.IsTarot;
        FinishTimeString = $"{FinishTime:yyyy.MM.dd HH:mm:ss}";
        Enemies = data.Enemies.SelectAsArray(EnemyView.Create);
        Avatars = data.Avatars.SelectAsArray(AvatarView.Create, context);
        ChoiceCards = data.ChoiceCards.SelectAsArray(BuffView.Create);
        SplendourSummary = data.SplendourBuff.Summary.Description.Replace("\\n", "\n", StringComparison.OrdinalIgnoreCase);
        SplendourBuffs = data.SplendourBuff.Buffs.SelectAsArray(SplendourBuffView.Create);
    }

    public string RoundId { get; }

    public bool IsGetMedal { get; }

    public bool IsTarot { get; }

    public string FinishTimeString { get; }

    public ImmutableArray<EnemyView> Enemies { get; }

    public ImmutableArray<AvatarView> Avatars { get; }

    public ImmutableArray<BuffView> ChoiceCards { get; }

    public string SplendourSummary { get; }

    public ImmutableArray<SplendourBuffView> SplendourBuffs { get; }

    internal DateTimeOffset FinishTime { get; }

    public static RoundView Create(RoleCombatRoundData data, TimeSpan offset, RoleCombatMetadataContext context)
    {
        return new(data, offset, context);
    }
}