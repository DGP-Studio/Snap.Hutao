// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.InterChange;

internal sealed class HutaoReserved
{
    public required uint Version { get; set; }

    public List<HutaoReservedEntry<HutaoReservedAchievement>>? Achievement { get; set; }

    public List<HutaoReservedEntry<Web.Enka.Model.AvatarInfo>>? AvatarInfo { get; set; }

    public List<HutaoReservedEntry<HutaoReservedCultivationEntry>>? Cultivation { get; set; }

    public List<HutaoReservedEntry<HutaoReservedSpiralAbyssEntry>>? SpiralAbyss { get; set; }
}