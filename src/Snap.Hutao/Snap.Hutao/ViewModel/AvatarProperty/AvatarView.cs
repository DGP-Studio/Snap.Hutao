﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.ViewModel.AvatarProperty;

internal sealed partial class AvatarView : INameIconSide,
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

    public List<ReliquaryView> Reliquaries { get; set; } = default!;

    public List<ConstellationView> Constellations { get; set; } = default!;

    public int ActivatedConstellationCount { get => Constellations.Where(c => c.IsActivated).Count(); }

    public List<SkillView> Skills { get; set; } = default!;

    public List<AvatarProperty> Properties { get; set; } = default!;

    public uint FetterLevel { get; set; }

    public string RefreshTimeFormat { get; set; } = default!;

    internal AvatarId Id { get; set; }

    internal uint LevelNumber { get; set; }

    public ICalculableAvatar ToCalculable()
    {
        return CalculableAvatar.From(this);
    }
}