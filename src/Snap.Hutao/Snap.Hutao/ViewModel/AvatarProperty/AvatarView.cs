// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.UI.Xaml.Data;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.AvatarProperty;

internal sealed partial class AvatarView : INameIconSide<Uri>,
    ICalculableSource<ICalculableAvatar>,
    IAdvancedCollectionViewItem
{
    public string Name { get; set; } = default!;

    public Uri Icon { get; set; } = default!;

    public Uri SideIcon { get; set; } = default!;

    public Uri NameCard { get; set; } = default!;

    public QualityType Quality { get; set; }

    public ElementType Element { get; set; }

    public string Level { get => LevelFormat.Format(LevelNumber); }

    public WeaponView? Weapon { get; set; }

    public RecommendPropertiesView RecommendedProperties { get; set; } = default!;

    public ImmutableArray<ReliquaryView> Reliquaries { get; set; }

    public ImmutableArray<ConstellationView> Constellations { get; set; }

    public int ActivatedConstellationCount { get => Constellations.Count(c => c.IsActivated); }

    public ImmutableArray<SkillView> Skills { get; set; }

    public ImmutableArray<AvatarProperty> Properties { get; set; }

    public uint FetterLevel { get; set; }

    public string FormattedRefreshTime { get; set; } = default!;

    public ImmutableArray<bool> PromoteList { get; set; }

    internal AvatarId Id { get; set; }

    internal uint LevelNumber { get; set; }

    internal PromoteLevel PromoteLevel { get; set; }

    public ICalculableAvatar ToCalculable()
    {
        return CalculableAvatar.From(this);
    }
}