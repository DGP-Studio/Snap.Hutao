// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.InterChange;

internal sealed class HutaoReservedSpiralAbyssEntry
{
    public required uint ScheduleId { get; set; }

    public required Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss SpiralAbyss { get; set; }
}