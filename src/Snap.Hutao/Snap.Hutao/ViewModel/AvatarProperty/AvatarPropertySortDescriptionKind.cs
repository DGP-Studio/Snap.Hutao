// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.AvatarProperty;

[ExtendedEnum]
internal enum AvatarPropertySortDescriptionKind
{
    [LocalizationKey(nameof(SH.ViewModelAvatarPropertySortDescriptionKindDefault))]
    Default,

    [LocalizationKey(nameof(SH.ViewModelAvatarPropertySortDescriptionKindLevelNumber))]
    LevelNumber,

    [LocalizationKey(nameof(SH.ViewModelAvatarPropertySortDescriptionKindQuality))]
    Quality,

    [LocalizationKey(nameof(SH.ViewModelAvatarPropertySortDescriptionKindActivatedConstellationCount))]
    ActivatedConstellationCount,

    [LocalizationKey(nameof(SH.ViewModelAvatarPropertySortDescriptionKindFetterLevel))]
    FetterLevel,

    [LocalizationKey(nameof(SH.ViewModelAvatarPropertySortDescriptionKindMaxHp))]
    MaxHp,

    [LocalizationKey(nameof(SH.ViewModelAvatarPropertySortDescriptionKindCurAttack))]
    CurAttack,

    [LocalizationKey(nameof(SH.ViewModelAvatarPropertySortDescriptionKindCurDefense))]
    CurDefense,

    [LocalizationKey(nameof(SH.ViewModelAvatarPropertySortDescriptionKindElementMastery))]
    ElementMastery,
}