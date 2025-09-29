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
    IPropertyValuesProvider
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

    public ImmutableArray<bool> PromoteArray { get; set; }

    public AvatarId Id { get; set; }

    public uint LevelNumber { get; set; }

    public int MaxHp { get => GetAvatarPropertyValue(FightProperty.FIGHT_PROP_MAX_HP); }

    public int CurAttack { get => GetAvatarPropertyValue(FightProperty.FIGHT_PROP_CUR_ATTACK); }

    public int CurDefense { get => GetAvatarPropertyValue(FightProperty.FIGHT_PROP_CUR_DEFENSE); }

    public int ElementMastery { get => GetAvatarPropertyValue(FightProperty.FIGHT_PROP_ELEMENT_MASTERY); }

    internal PromoteLevel PromoteLevel { get; set; }

    public ICalculableAvatar ToCalculable()
    {
        return CalculableAvatar.From(this);
    }

    private int GetAvatarPropertyValue(FightProperty fightProperty)
    {
        AvatarProperty property = Properties.Single(p => p.FightProperty == fightProperty);
        int value = int.Parse(property.Value);
        if (!string.IsNullOrEmpty(property.AddValue))
        {
            value += int.Parse(property.AddValue);
        }

        return value;
    }
}