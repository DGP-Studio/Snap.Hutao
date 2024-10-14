// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata.Weapon;

internal sealed class LevelDescription
{
    public int Level { get; set; } = default!;

    [JsonIgnore]
    public string LevelFormatted { get => SH.FormatModelWeaponAffixFormat(Level + 1); }

    public string Description { get; set; } = default!;
}