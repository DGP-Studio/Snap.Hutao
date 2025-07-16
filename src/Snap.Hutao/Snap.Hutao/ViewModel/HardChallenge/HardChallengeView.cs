// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;
using System.Collections.Immutable;
using MetadataHardChallengeSchedule = Snap.Hutao.Model.Metadata.HardChallengeSchedule;

namespace Snap.Hutao.ViewModel.HardChallenge;

internal sealed partial class HardChallengeView : IEntityAccess<HardChallengeEntry?>, IAdvancedCollectionViewItem
{
    private HardChallengeView(HardChallengeEntry entity, HardChallengeMetadataContext context)
        : this(context.IdHardChallengeScheduleMap[entity.ScheduleId], context)
    {
        Entity = entity;

        HardChallengeData hardChallengeData = entity.HardChallengeData;

        SinglePlayer = DataEntryView.Create(hardChallengeData.SinglePlayer, context);
        MultiPlayer = DataEntryView.Create(hardChallengeData.MultiPlayer, context);

        BlingAvatars = hardChallengeData.Blings.SelectAsArray(AvatarBling.Create, context);
        Engaged = true;
    }

    private HardChallengeView(MetadataHardChallengeSchedule hardChallengeSchedule, HardChallengeMetadataContext context)
    {
        ScheduleId = hardChallengeSchedule.Id;
        ScheduleName = hardChallengeSchedule.Name;
        FormattedTime = $"{hardChallengeSchedule.Begin:yyyy.MM.dd HH:mm} - {hardChallengeSchedule.End:yyyy.MM.dd HH:mm}";
    }

    public uint ScheduleId { get; }

    public string ScheduleName { get; }

    public string Schedule { get => SH.FormatModelEntityHardChallengeScheduleFormat(ScheduleId, ScheduleName); }

    public string FormattedTime { get; }

    public bool Engaged { get; }

    public HardChallengeEntry? Entity { get; }

    public DataEntryView? SinglePlayer { get; }

    public DataEntryView? MultiPlayer { get; }

    public ImmutableArray<AvatarBling> BlingAvatars { get; } = [];

    public static HardChallengeView Create(HardChallengeEntry entity, HardChallengeMetadataContext context)
    {
        return new(entity, context);
    }

    public static HardChallengeView Create(HardChallengeEntry? entity, MetadataHardChallengeSchedule meta, HardChallengeMetadataContext context)
    {
        return entity is not null ? new(entity, context) : new(meta, context);
    }
}