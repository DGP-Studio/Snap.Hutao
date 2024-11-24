// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

namespace Snap.Hutao.ViewModel.RoleCombat;

internal sealed class RoundView
{
    private RoundView(RoleCombatRoundData data, in TimeSpan offset, RoleCombatMetadataContext context)
    {
        FinishTime = DateTimeOffset.FromUnixTimeSeconds(data.FinishTime).ToOffset(offset);

        RoundId = SH.FormatViewModelRoleCombatRound(data.RoundId);
        IsGetMedal = data.IsGetMedal;
        FinishTimeString = $"{FinishTime:yyyy.MM.dd HH:mm:ss}";
        Enemies = data.Enemies.Select(EnemyView.From).ToList();
        Avatars = data.Avatars.Select(avatar => AvatarView.From(avatar, context.IdAvatarMap[avatar.AvatarId])).ToList();
        ChoiceCards = data.ChoiceCards.Select(BuffView.From).ToList();
        SplendourSummary = data.SplendourBuff.Summary.Description;
        SplendourBuffs = data.SplendourBuff.Buffs.Select(SplendourBuffView.From).ToList();
    }

    public string RoundId { get; }

    public bool IsGetMedal { get; }

    public string FinishTimeString { get; }

    public List<EnemyView> Enemies { get; }

    public List<AvatarView> Avatars { get; }

    public List<BuffView> ChoiceCards { get; }

    public string SplendourSummary { get; }

    public List<SplendourBuffView> SplendourBuffs { get; }

    internal DateTimeOffset FinishTime { get; }

    public static RoundView From(RoleCombatRoundData data, in TimeSpan offset, RoleCombatMetadataContext context)
    {
        return new(data, offset, context);
    }
}