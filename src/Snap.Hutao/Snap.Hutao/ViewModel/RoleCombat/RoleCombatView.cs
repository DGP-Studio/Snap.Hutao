// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;
using RoleCombatSchedule = Snap.Hutao.Model.Metadata.RoleCombatSchedule;

namespace Snap.Hutao.ViewModel.RoleCombat;

internal sealed partial class RoleCombatView : IEntityAccess<RoleCombatEntry?>, IAdvancedCollectionViewItem
{
    private readonly RoleCombatEntry? entity;

    private RoleCombatView(RoleCombatEntry entity, RoleCombatMetadataContext context)
        : this(context.IdRoleCombatScheduleMap[entity.ScheduleId], context)
    {
        this.entity = entity;

        TimeSpan offset = PlayerUid.GetRegionTimeZoneUtcOffsetForUid(entity.Uid);

        RoleCombatData roleCombatData = entity.RoleCombatData;

        Stat = roleCombatData.Stat;
        Difficulty = roleCombatData.Stat.DifficultyId.GetLocalizedDescription();
        FormattedHeraldry = $"已抵达{Difficulty}{MaxRound}";
        BackupAvatars = roleCombatData.Detail.BackupAvatars
            .OrderByDescending(avatar => avatar.AvatarType)
            .ThenByDescending(avatar => avatar.Rarity)
            .ThenByDescending(avatar => avatar.Element)
            .ThenByDescending(avatar => avatar.AvatarId.Value) // TODO: IdentityStruct IComparable?
            .Select(avatar => AvatarView.From(avatar, context.IdAvatarMap[avatar.AvatarId]))
            .ToList();
        Rounds = roleCombatData.Detail.RoundsData.SelectList(r => RoundView.From(r, offset, context));
        TotalBattleTimes = roleCombatData.Detail.FightStatistic.TotalUseTime;

        if (roleCombatData.Detail.FightStatistic.IsShowBattleStats)
        {
            Shortest = ToAvatarDamages(roleCombatData.Detail.FightStatistic.ShortestAvatarList, context);
            Defeat = ToAvatarDamage(roleCombatData.Detail.FightStatistic.MaxDefeatAvatar, context);
            Damage = ToAvatarDamage(roleCombatData.Detail.FightStatistic.MaxDamageAvatar, context);
            TakeDamage = ToAvatarDamage(roleCombatData.Detail.FightStatistic.MaxTakeDamageAvatar, context);
        }

        Engaged = true;
    }

    private RoleCombatView(RoleCombatSchedule roleCombatSchedule, RoleCombatMetadataContext context)
    {
        ScheduleId = roleCombatSchedule.Id;
        TimeFormatted = $"{roleCombatSchedule.Begin:yyyy.MM.dd HH:mm} - {roleCombatSchedule.End:yyyy.MM.dd HH:mm}";

        Elements = roleCombatSchedule.Elements;
        SpecialAvatars = roleCombatSchedule.SpecialAvatars.SelectList(id => AvatarView.From(context.IdAvatarMap[id]));
        InitialAvatars = roleCombatSchedule.InitialAvatars.SelectList(id => AvatarView.From(context.IdAvatarMap[id]));
    }

    public uint ScheduleId { get; }

    public string Schedule { get => SH.FormatModelEntitySpiralAbyssScheduleFormat(ScheduleId); }

    public RoleCombatEntry? Entity { get => entity; }

    public string TimeFormatted { get; }

    public List<ElementType> Elements { get; }

    public List<AvatarView> SpecialAvatars { get; }

    public List<AvatarView> InitialAvatars { get; }

    public bool Engaged { get; }

    public RoleCombatStat? Stat { get; }

    public string Difficulty { get; } = default!;

    public string FormattedHeraldry { get; } = default!;

    public List<AvatarView> BackupAvatars { get; } = [];

    public List<RoundView> Rounds { get; } = [];

    public string MaxRound { get => Stat is not null ? $"第 {Stat.MaxRoundId} 幕" : default!; }

    public int TotalBattleTimes { get; }

    public string TotalBattleTimesFormatted { get => $"{TimeSpan.FromSeconds(TotalBattleTimes):hh':'mm':'ss}"; }

    public List<AvatarDamage> Shortest { get; } = default!;

    public AvatarDamage? Defeat { get; }

    public AvatarDamage? Damage { get; }

    public AvatarDamage? TakeDamage { get; }

    public static RoleCombatView From(RoleCombatEntry entity, RoleCombatMetadataContext context)
    {
        return new(entity, context);
    }

    public static RoleCombatView From(RoleCombatEntry? entity, RoleCombatSchedule meta, RoleCombatMetadataContext context)
    {
        return entity is not null ? new(entity, context) : new(meta, context);
    }

    private static List<AvatarDamage> ToAvatarDamages(List<RoleCombatAvatarDamage> avatarDamages, RoleCombatMetadataContext context)
    {
        return avatarDamages.Select(r => new AvatarDamage(r.Value, context.IdAvatarMap[r.AvatarId])).ToList();
    }

    private static AvatarDamage? ToAvatarDamage(RoleCombatAvatarDamage avatarDamage, RoleCombatMetadataContext context)
    {
        return new AvatarDamage(avatarDamage.Value, context.IdAvatarMap[avatarDamage.AvatarId]);
    }
}