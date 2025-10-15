// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;
using System.Collections.Immutable;
using System.Globalization;
using MetadataRoleCombatSchedule = Snap.Hutao.Model.Metadata.RoleCombatSchedule;

namespace Snap.Hutao.ViewModel.RoleCombat;

internal sealed partial class RoleCombatView : IEntityAccess<RoleCombatEntry?>, IPropertyValuesProvider
{
    private RoleCombatView(RoleCombatEntry entity, RoleCombatMetadataContext context)
        : this(entity, context.IdRoleCombatScheduleMap[entity.ScheduleId], context)
    {
    }

    private RoleCombatView(RoleCombatEntry entity, MetadataRoleCombatSchedule roleCombatSchedule, RoleCombatMetadataContext context)
        : this(roleCombatSchedule, context)
    {
        Entity = entity;

        RoleCombatData roleCombatData = entity.RoleCombatData;
        Stat = roleCombatData.Stat;
        Difficulty = roleCombatData.Stat.DifficultyId.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture);
        FormattedHeraldry = SH.FormatViewModelRoleCombatHeraldry(MaxRound);
        BackupAvatars =
        [
            .. roleCombatData.Detail.BackupAvatars
                .OrderByDescending(static avatar => avatar.AvatarType)
                .ThenByDescending(static avatar => avatar.Rarity)
                .ThenByDescending(static avatar => avatar.Element)
                .ThenByDescending(static avatar => avatar.AvatarId.Value)
                .Select(avatar => AvatarView.Create(avatar, context))
        ];
        TimeSpan offset = PlayerUid.GetRegionTimeZoneUtcOffsetForUid(entity.Uid);
        Rounds = roleCombatData.Detail.RoundsData.SelectAsArray(static (data, state) => RoundView.Create(data, state.offset, state.context), (offset, context));
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

    private RoleCombatView(MetadataRoleCombatSchedule roleCombatSchedule, RoleCombatMetadataContext context)
    {
        ScheduleId = roleCombatSchedule.Id;
        FormattedTime = $"{roleCombatSchedule.Begin:yyyy.MM.dd HH:mm} - {roleCombatSchedule.End:yyyy.MM.dd HH:mm}";

        Elements = roleCombatSchedule.Elements;
        SpecialAvatars = roleCombatSchedule.SpecialAvatars.SelectAsArray(static (id, context) => AvatarView.Create(context.GetAvatar(id)), context);
        InitialAvatars = roleCombatSchedule.InitialAvatars.SelectAsArray(static (id, context) => AvatarView.Create(context.GetAvatar(id)), context);
    }

    public uint ScheduleId { get; }

    public string Schedule { get => SH.FormatModelEntitySpiralAbyssSchedule(ScheduleId); }

    public string FormattedTime { get; }

    public bool Engaged { get; }

    public RoleCombatEntry? Entity { get; }

    public int TotalBattleTimes { get; }

    public string? Difficulty { get; }

    public string? FormattedHeraldry { get; }

    public string? MaxRound
    {
        get
        {
            if (Stat is null)
            {
                return default;
            }

            return Stat.TarotFinishedCount > 0
                ? SH.FormatViewModelRoleCombatRoundAndTarot(Stat.MaxRoundId, Stat.TarotFinishedCount)
                : SH.FormatViewModelRoleCombatRound(Stat.MaxRoundId);
        }
    }

    public string FormattedTotalBattleTimes { get => $"{TimeSpan.FromSeconds(TotalBattleTimes):hh':'mm':'ss}"; }

    public RoleCombatStat? Stat { get; }

    public ImmutableArray<ElementType> Elements { get; }

    public ImmutableArray<AvatarView> SpecialAvatars { get; }

    public ImmutableArray<AvatarView> InitialAvatars { get; }

    public ImmutableArray<AvatarView> BackupAvatars { get; } = [];

    public ImmutableArray<RoundView> Rounds { get; } = [];

    public ImmutableArray<AvatarDamage> Shortest { get; } = [];

    public AvatarDamage? Defeat { get; }

    public AvatarDamage? Damage { get; }

    public AvatarDamage? TakeDamage { get; }

    public static RoleCombatView Create(RoleCombatEntry entity, RoleCombatMetadataContext context)
    {
        return new(entity, context);
    }

    public static RoleCombatView Create(RoleCombatEntry? entity, MetadataRoleCombatSchedule meta, RoleCombatMetadataContext context)
    {
        return entity is not null ? new(entity, meta, context) : new(meta, context);
    }

    private static ImmutableArray<AvatarDamage> ToAvatarDamages(ImmutableArray<RoleCombatAvatarStatistics> avatarDamages, RoleCombatMetadataContext context)
    {
        return avatarDamages.SelectAsArray(static (r, context) => new AvatarDamage(r.Value, context.GetAvatar(r.AvatarId)), context);
    }

    private static AvatarDamage? ToAvatarDamage(RoleCombatAvatarStatistics? avatarDamage, RoleCombatMetadataContext context)
    {
        if (avatarDamage is not { AvatarId.Value: not 0U })
        {
            return default;
        }

        return new(avatarDamage.Value, context.IdAvatarMap[avatarDamage.AvatarId]);
    }
}