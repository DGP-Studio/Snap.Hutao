// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

[Table("spiral_abysses")]
internal sealed class SpiralAbyssEntry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    public uint ScheduleId { get; set; }

    public string Uid { get; set; } = default!;

    public Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss SpiralAbyss { get; set; } = default!;

    public static SpiralAbyssEntry Create(string uid, Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss spiralAbyss)
    {
        return new()
        {
            Uid = uid,
            ScheduleId = spiralAbyss.ScheduleId,
            SpiralAbyss = spiralAbyss,
        };
    }
}