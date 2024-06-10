// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.Complex;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.ViewModel.Wiki;

namespace Snap.Hutao.Model.Metadata.Avatar;

[HighQuality]
internal partial class Avatar : INameQualityAccess,
    IStatisticsItemConvertible,
    ISummaryItemConvertible,
    IItemConvertible,
    ICalculableSource<ICalculableAvatar>,
    ICultivationItemsAccess
{
    public AvatarId Id { get; set; }

    public PromoteId PromoteId { get; set; }

    public uint Sort { get; set; }

    public BodyType Body { get; set; } = default!;

    public string Icon { get; set; } = default!;

    public string SideIcon { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;

    public DateTimeOffset BeginTime { get; set; }

    public QualityType Quality { get; set; }

    public WeaponType Weapon { get; set; }

    public AvatarBaseValue BaseValue { get; set; } = default!;

    public List<TypeValue<FightProperty, GrowCurveType>> GrowCurves { get; set; } = default!;

    public SkillDepot SkillDepot { get; set; } = default!;

    public FetterInfo FetterInfo { get; set; } = default!;

    public List<Costume> Costumes { get; set; } = default!;

    public List<MaterialId> CultivationItems { get; set; } = default!;

    [JsonIgnore]
    public AvatarCollocationView? CollocationView { get; set; }

    [JsonIgnore]
    public CookBonusView? CookBonusView { get; set; }

    [JsonIgnore]
    public List<Material>? CultivationItemsView { get; set; }

    [SuppressMessage("", "CA1822")]
    public uint MaxLevel { get => GetMaxLevel(); }

    public static uint GetMaxLevel()
    {
        return 90U;
    }

    public ICalculableAvatar ToCalculable()
    {
        return CalculableAvatar.From(this);
    }

    public Model.Item ToItem()
    {
        return new()
        {
            Name = Name,
            Icon = AvatarIconConverter.IconNameToUri(Icon),
            Badge = ElementNameIconConverter.ElementNameToIconUri(FetterInfo.VisionBefore),
            Quality = Quality,
        };
    }

    public StatisticsItem ToStatisticsItem(int count)
    {
        return new()
        {
            Name = Name,
            Icon = AvatarIconConverter.IconNameToUri(Icon),
            Badge = ElementNameIconConverter.ElementNameToIconUri(FetterInfo.VisionBefore),
            Quality = Quality,

            Count = count,
        };
    }

    public SummaryItem ToSummaryItem(int lastPull, in DateTimeOffset time, bool isUp)
    {
        return new()
        {
            Name = Name,
            Icon = AvatarIconConverter.IconNameToUri(Icon),
            Badge = ElementNameIconConverter.ElementNameToIconUri(FetterInfo.VisionBefore),
            Quality = Quality,

            Time = time,
            LastPull = lastPull,
            IsUp = isUp,
        };
    }
}