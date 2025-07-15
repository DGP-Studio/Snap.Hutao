// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;
using MetadataHardChallengeSchedule = Snap.Hutao.Model.Metadata.HardChallengeSchedule;

namespace Snap.Hutao.ViewModel.HardChallenge;

internal sealed partial class HardChallengeView : IEntityAccess<HardChallengeEntry?>, IAdvancedCollectionViewItem
{
    private HardChallengeView(HardChallengeEntry entity, HardChallengeMetadataContext context)
        : this(context.IdHardChallengeScheduleMap[entity.ScheduleId], context)
    {
        Entity = entity;

        TimeSpan offset = PlayerUid.GetRegionTimeZoneUtcOffsetForUid(entity.Uid);

        HardChallengeData roleCombatData = entity.HardChallengeData;

        Stat = roleCombatData.Stat;
        Difficulty = roleCombatData.Stat.DifficultyId.GetLocalizedDescription();
        FormattedHeraldry = SH.FormatViewModelRoleCombatHeraldry(MaxRound);
        BackupAvatars =
        [
            .. roleCombatData.Detail.BackupAvatars
                .OrderByDescending(static avatar => avatar.AvatarType)
                .ThenByDescending(static avatar => avatar.Rarity)
                .ThenByDescending(static avatar => avatar.Element)
                .ThenByDescending(static avatar => avatar.AvatarId.Value)
                .Select(avatar => AvatarView.Create(avatar, context.IdAvatarMap[avatar.AvatarId]))
        ];
        Rounds = roleCombatData.Detail.RoundsData.SelectAsArray(static (r, state) => RoundView.Create(r, state.offset, state.context), (offset, context));
        TotalBattleTimes = roleCombatData.Detail.FightStatistics.TotalUseTime;

        if (roleCombatData.Detail.FightStatistics.IsShowBattleStats)
        {
            Shortest = ToAvatarDamages(roleCombatData.Detail.FightStatistics.ShortestAvatarList, context);
            Defeat = ToAvatarDamage(roleCombatData.Detail.FightStatistics.MaxDefeatAvatar, context);
            Damage = ToAvatarDamage(roleCombatData.Detail.FightStatistics.MaxDamageAvatar, context);
            TakeDamage = ToAvatarDamage(roleCombatData.Detail.FightStatistics.MaxTakeDamageAvatar, context);
        }

        Engaged = true;
    }

    private HardChallengeView(MetadataHardChallengeSchedule hardChallengeSchedule, HardChallengeMetadataContext context)
    {
        ScheduleId = hardChallengeSchedule.Id;
        FormattedTime = $"{hardChallengeSchedule.Begin:yyyy.MM.dd HH:mm} - {hardChallengeSchedule.End:yyyy.MM.dd HH:mm}";
    }

    public uint ScheduleId { get; }

    public string ScheduleName { get; }

    public string Schedule { get => SH.FormatModelEntityHardChallengeScheduleFormat(ScheduleId, ScheduleName); }

    public HardChallengeEntry? Entity { get; }

    public string FormattedTime { get; }

    public static HardChallengeView Create(HardChallengeEntry entity, HardChallengeMetadataContext context)
    {
        return new(entity, context);
    }

    public static HardChallengeView Create(HardChallengeEntry? entity, MetadataHardChallengeSchedule meta, HardChallengeMetadataContext context)
    {
        return entity is not null ? new(entity, context) : new(meta, context);
    }
}