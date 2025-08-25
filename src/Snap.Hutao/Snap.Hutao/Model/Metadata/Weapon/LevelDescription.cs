// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata.Weapon;

internal sealed class LevelDescription
{
    public required int Level { get; init; }

    [JsonIgnore]
    public string FormattedLevel { get => SH.FormatModelWeaponAffix(Level + 1); }

    public required string Description { get; init; }
}