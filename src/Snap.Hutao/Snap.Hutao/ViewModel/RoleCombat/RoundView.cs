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

    public string RoundId { get; set; }

    public bool IsGetMedal { get; set; }

    public string FinishTimeString { get; set; }

    public List<EnemyView> Enemies { get; set; }

    public List<AvatarView> Avatars { get; set; }

    public List<BuffView> ChoiceCards { get; set; }

    public string SplendourSummary { get; set; }

    public List<SplendourBuffView> SplendourBuffs { get; set; }

    internal DateTimeOffset FinishTime { get; set; }

    public static RoundView From(RoleCombatRoundData data, in TimeSpan offset, RoleCombatMetadataContext context)
    {
        return new(data, offset, context);
    }
}