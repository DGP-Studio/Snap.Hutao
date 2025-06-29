// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;
using System.Collections.Immutable;
using RoleCombatSchedule = Snap.Hutao.Model.Metadata.RoleCombatSchedule;

namespace Snap.Hutao.ViewModel.RoleCombat;

internal sealed partial class RoleCombatView : IEntityAccess<RoleCombatEntry?>, IAdvancedCollectionViewItem
{
    private RoleCombatView(RoleCombatEntry entity, RoleCombatMetadataContext context)
        : this(context.IdRoleCombatScheduleMap[entity.ScheduleId], context)
    {
        Entity = entity;

        TimeSpan offset = PlayerUid.GetRegionTimeZoneUtcOffsetForUid(entity.Uid);

        RoleCombatData roleCombatData = entity.RoleCombatData;

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
        Rounds = [.. roleCombatData.Detail.RoundsData.Select(r => RoundView.From(r, offset, context))];
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

    private RoleCombatView(RoleCombatSchedule roleCombatSchedule, RoleCombatMetadataContext context)
    {
        ScheduleId = roleCombatSchedule.Id;
        FormattedTime = $"{roleCombatSchedule.Begin:yyyy.MM.dd HH:mm} - {roleCombatSchedule.End:yyyy.MM.dd HH:mm}";

        Elements = roleCombatSchedule.Elements;
        SpecialAvatars = roleCombatSchedule.SpecialAvatars.SelectAsArray(static (id, map) => AvatarView.Create(map[id]), context.IdAvatarMap);
        InitialAvatars = roleCombatSchedule.InitialAvatars.SelectAsArray(static (id, map) => AvatarView.Create(map[id]), context.IdAvatarMap);
    }

    public uint ScheduleId { get; }

    public string Schedule { get => SH.FormatModelEntitySpiralAbyssScheduleFormat(ScheduleId); }

    public RoleCombatEntry? Entity { get; }

    public string FormattedTime { get; }

    public ImmutableArray<ElementType> Elements { get; }

    public ImmutableArray<AvatarView> SpecialAvatars { get; }

    public ImmutableArray<AvatarView> InitialAvatars { get; }

    public bool Engaged { get; }

    public RoleCombatStat? Stat { get; }

    public string Difficulty { get; } = default!;

    public string FormattedHeraldry { get; } = default!;

    public ImmutableArray<AvatarView> BackupAvatars { get; } = [];

    public ImmutableArray<RoundView> Rounds { get; } = [];

    public string MaxRound { get => Stat is not null ? SH.FormatViewModelRoleCombatRound(Stat.MaxRoundId) : default!; }

    public int TotalBattleTimes { get; }

    public string FormattedTotalBattleTimes { get => $"{TimeSpan.FromSeconds(TotalBattleTimes):hh':'mm':'ss}"; }

    public ImmutableArray<AvatarDamage> Shortest { get; }

    public AvatarDamage? Defeat { get; }

    public AvatarDamage? Damage { get; }

    public AvatarDamage? TakeDamage { get; }

    public static RoleCombatView Create(RoleCombatEntry entity, RoleCombatMetadataContext context)
    {
        return new(entity, context);
    }

    public static RoleCombatView Create(RoleCombatEntry? entity, RoleCombatSchedule meta, RoleCombatMetadataContext context)
    {
        return entity is not null ? new(entity, context) : new(meta, context);
    }

    private static ImmutableArray<AvatarDamage> ToAvatarDamages(ImmutableArray<RoleCombatAvatarStatistics> avatarDamages, RoleCombatMetadataContext context)
    {
        return avatarDamages.SelectAsArray(static (r, map) => new AvatarDamage(r.Value, map[r.AvatarId]), context.IdAvatarMap);
    }

    private static AvatarDamage? ToAvatarDamage(RoleCombatAvatarStatistics avatarDamage, RoleCombatMetadataContext context)
    {
        if (avatarDamage is not { AvatarId.Value: not 0U })
        {
            return default;
        }

        return new(avatarDamage.Value, context.IdAvatarMap[avatarDamage.AvatarId]);
    }
}