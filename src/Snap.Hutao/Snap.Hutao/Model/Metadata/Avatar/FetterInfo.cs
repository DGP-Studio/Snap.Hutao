// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Avatar;

internal sealed class FetterInfo
{
    public string Title { get; set; } = default!;

    public string Detail { get; set; } = default!;

    public AssociationType Association { get; set; } = default!;

    public string Native { get; set; } = default!;

    public uint BirthMonth { get; set; }

    public uint BirthDay { get; set; }

    [JsonIgnore]
    public string BirthFormatted
    {
        get => SH.FormatModelMetadataFetterInfoBirthdayFormat(BirthMonth, BirthDay);
    }

    public string VisionBefore { get; set; } = default!;

    public string ConstellationBefore { get; set; } = default!;

    public string ConstellationAfter { get; set; } = default!;

    [JsonIgnore]
    public string Constellation
    {
        get
        {
            return string.IsNullOrEmpty(ConstellationAfter)
                ? ConstellationBefore
                : ConstellationAfter;
        }
    }

    public string CvChinese { get; set; } = default!;

    public string CvJapanese { get; set; } = default!;

    public string CvEnglish { get; set; } = default!;

    public string CvKorean { get; set; } = default!;

    public CookBonus? CookBonus { get; set; }

    public ImmutableArray<Fetter> Fetters { get; set; } = default!;

    public ImmutableArray<Fetter> FetterStories { get; set; } = default!;
}