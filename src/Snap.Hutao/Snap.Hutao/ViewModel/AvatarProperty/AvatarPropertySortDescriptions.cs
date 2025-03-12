// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Collections;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.AvatarProperty;

[SuppressMessage("", "SA1202")]
internal static class AvatarPropertySortDescriptions
{
    private static readonly SortDescription LevelNumber = new(nameof(AvatarView.LevelNumber), SortDirection.Descending);
    private static readonly SortDescription Quality = new(nameof(AvatarView.Quality), SortDirection.Descending);
    private static readonly SortDescription Element = new(nameof(AvatarView.Element), SortDirection.Descending);
    private static readonly SortDescription Id = new(nameof(AvatarView.Id), SortDirection.Descending);
    private static readonly SortDescription ActivatedConstellationCount = new(nameof(AvatarView.ActivatedConstellationCount), SortDirection.Descending);
    private static readonly SortDescription FetterLevel = new(nameof(AvatarView.FetterLevel), SortDirection.Descending);
    private static readonly SortDescription MaxHp = new(nameof(AvatarView.MaxHp), SortDirection.Descending);
    private static readonly SortDescription CurAttack = new(nameof(AvatarView.CurAttack), SortDirection.Descending);
    private static readonly SortDescription CurDefense = new(nameof(AvatarView.CurDefense), SortDirection.Descending);
    private static readonly SortDescription ElementMastery = new(nameof(AvatarView.ElementMastery), SortDirection.Descending);

    private static readonly ImmutableArray<SortDescription> DefaultSortDescriptions = [];

    private static readonly ImmutableArray<SortDescription> LevelNumberSortDescriptions =
    [
        LevelNumber,
        Quality,
        Element,
        Id
    ];

    private static readonly ImmutableArray<SortDescription> QualitySortDescriptions =
    [
        Quality,
        LevelNumber,
        Element,
        Id
    ];

    private static readonly ImmutableArray<SortDescription> ActivatedConstellationCountSortDescriptions =
    [
        ActivatedConstellationCount,
        LevelNumber,
        Quality,
        Element,
        Id
    ];

    private static readonly ImmutableArray<SortDescription> FetterLevelSortDescriptions =
    [
        FetterLevel,
        LevelNumber,
        Quality,
        Element,
        Id
    ];

    private static readonly ImmutableArray<SortDescription> MaxHpSortDescriptions =
    [
        MaxHp,
        LevelNumber,
        Quality,
        Element,
        Id
    ];

    private static readonly ImmutableArray<SortDescription> CurAttackSortDescriptions =
    [
        CurAttack,
        LevelNumber,
        Quality,
        Element,
        Id
    ];

    private static readonly ImmutableArray<SortDescription> CurDefenseSortDescriptions =
    [
        CurDefense,
        LevelNumber,
        Quality,
        Element,
        Id
    ];

    private static readonly ImmutableArray<SortDescription> ElementMasterySortDescriptions =
    [
        ElementMastery,
        LevelNumber,
        Quality,
        Element,
        Id
    ];

    public static ImmutableArray<SortDescription> Get(AvatarPropertySortDescriptionKind kind)
    {
        return kind switch
        {
            AvatarPropertySortDescriptionKind.LevelNumber => LevelNumberSortDescriptions,
            AvatarPropertySortDescriptionKind.Quality => QualitySortDescriptions,
            AvatarPropertySortDescriptionKind.ActivatedConstellationCount => ActivatedConstellationCountSortDescriptions,
            AvatarPropertySortDescriptionKind.FetterLevel => FetterLevelSortDescriptions,
            AvatarPropertySortDescriptionKind.MaxHp => MaxHpSortDescriptions,
            AvatarPropertySortDescriptionKind.CurAttack => CurAttackSortDescriptions,
            AvatarPropertySortDescriptionKind.CurDefense => CurDefenseSortDescriptions,
            AvatarPropertySortDescriptionKind.ElementMastery => ElementMasterySortDescriptions,
            _ => DefaultSortDescriptions,
        };
    }
}