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
    public string BirthFormatted { get => SH.FormatModelMetadataFetterInfoBirthday(BirthMonth, BirthDay); }

    public required string VisionBefore { get; init; }

    public string? VisionAfter { get; init; }

    public string Vision
    {
        get => string.IsNullOrEmpty(VisionAfter) ? VisionBefore : VisionAfter;
    }

    public string? VisionOverrideLocked { get; init; }

    public string? VisionOverrideUnlocked { get; init; }

    [field: MaybeNull]
    public string VisionOverride
    {
        get
        {
            if (!string.IsNullOrEmpty(field))
            {
                return field;
            }

            if (!string.IsNullOrEmpty(VisionOverrideUnlocked))
            {
                return field = VisionOverrideUnlocked;
            }

            if (!string.IsNullOrEmpty(VisionOverrideLocked))
            {
                return field = VisionOverrideLocked;
            }

            return field = SH.ViewPageWiKiAvatarVisionTitle;
        }
        set;
    }

    public required string ConstellationBefore { get; init; }

    public string? ConstellationAfter { get; init; }

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