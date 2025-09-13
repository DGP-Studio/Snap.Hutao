// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.ViewModel.Complex;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.ViewModel.Wiki;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Snap.Hutao.Model.Metadata.Avatar;

[DebuggerDisplay("Name={Name},Id={Id}")]
internal partial class Avatar : IDefaultIdentity<AvatarId>,
    INameQualityAccess,
    IStatisticsItemConvertible,
    ISummaryItemConvertible,
    IItemConvertible,
    ICalculableSource<ICalculableAvatar>,
    ICultivationItemsAccess,
    IPropertyValuesProvider,
    IJsonOnDeserialized
{
    public required AvatarId Id { get; init; }

    public required PromoteId PromoteId { get; init; }

    public required uint Sort { get; init; }

    public required BodyType Body { get; init; }

    public required string Icon { get; init; }

    public required string SideIcon { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required DateTimeOffset BeginTime { get; init; }

    public required QualityType Quality { get; init; }

    public required WeaponType Weapon { get; init; }

    public required AvatarBaseValue BaseValue { get; init; }

    public required TypeValueCollection<FightProperty, GrowCurveType> GrowCurves { get; init; }

    public required SkillDepot SkillDepot { get; init; }

    public required FetterInfo FetterInfo { get; init; }

    public required ImmutableArray<Costume> Costumes { get; init; }

    public required ImmutableArray<MaterialId> CultivationItems { get; init; }

    public required AvatarNameCard NameCard { get; init; }

    public TraceEffect? TraceEffect { get; init; }

    [JsonIgnore]
    public AvatarCollocationView? CollocationView { get; set; }

    [JsonIgnore]
    public CookBonusView? CookBonusView { get; set; }

    [JsonIgnore]
    public ImmutableArray<Material>? CultivationItemsView { get; set; }

    [JsonIgnore]
    public IAdvancedCollectionView<Costume>? CostumesView { get; set; }

    [SuppressMessage("", "CA1822")]
    public uint MaxLevel { get => GetMaxLevel(); }

    public static uint GetMaxLevel()
    {
        return 100U;
    }

    public ICalculableAvatar ToCalculable()
    {
        return CalculableAvatar.From(this);
    }

    public TItem ToItem<TItem>()
        where TItem : Model.Item, new()
    {
        return new()
        {
            Id = Id,
            Name = Name,
            Icon = AvatarIconConverter.IconNameToUri(Icon),
            Badge = ElementNameIconConverter.ElementNameToUri(FetterInfo.VisionBefore),
            Quality = Quality,
        };
    }

    public StatisticsItem ToStatisticsItem(int count)
    {
        return new()
        {
            Id = Id,
            Name = Name,
            Icon = AvatarIconConverter.IconNameToUri(Icon),
            Badge = ElementNameIconConverter.ElementNameToUri(FetterInfo.VisionBefore),
            Quality = Quality,

            Count = count,
        };
    }

    public SummaryItem ToSummaryItem(int lastPull, in DateTimeOffset time, bool isUp)
    {
        return new()
        {
            Id = Id,
            Name = Name,
            Icon = AvatarIconConverter.IconNameToUri(Icon),
            Badge = ElementNameIconConverter.ElementNameToUri(FetterInfo.VisionBefore),
            Quality = Quality,

            Time = time,
            LastPull = lastPull,
            IsUp = isUp,
        };
    }

    public void OnDeserialized()
    {
        if (AvatarIds.UsesGnosis(Id))
        {
            FetterInfo.VisionOverride = SH.ViewPageWiKiAvatarGnosisTitle;
        }
    }
}