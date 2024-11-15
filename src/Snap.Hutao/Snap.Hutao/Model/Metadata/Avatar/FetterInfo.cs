// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Avatar;

internal sealed class FetterInfo
{
    public required string Title { get; init; }

    public required string Detail { get; init; }

    public required AssociationType Association { get; init; }

    public required string Native { get; init; }

    public required uint BirthMonth { get; init; }

    public required uint BirthDay { get; init; }

    [JsonIgnore]
    public string BirthFormatted { get => SH.FormatModelMetadataFetterInfoBirthdayFormat(BirthMonth, BirthDay); }

    public required string VisionBefore { get; init; }

    public required string ConstellationBefore { get; init; }

    public required string ConstellationAfter { get; init; }

    [JsonIgnore]
    public string Constellation { get => string.IsNullOrEmpty(ConstellationAfter) ? ConstellationBefore : ConstellationAfter; }

    public required string CvChinese { get; init; }

    public required string CvJapanese { get; init; }

    public required string CvEnglish { get; init; }

    public required string CvKorean { get; init; }

    public CookBonus? CookBonus { get; init; }

    public required ImmutableArray<Fetter> Fetters { get; init; }

    public required ImmutableArray<Fetter> FetterStories { get; init; }
}