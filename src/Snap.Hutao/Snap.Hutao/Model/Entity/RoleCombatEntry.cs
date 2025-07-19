// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

[Table("role_combats")]
internal sealed class RoleCombatEntry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    public uint ScheduleId { get; set; }

    public string Uid { get; set; } = default!;

    public RoleCombatData RoleCombatData { get; set; } = default!;

    public static RoleCombatEntry Create(string uid, RoleCombatData roleCombatData)
    {
        return new()
        {
            Uid = uid,
            ScheduleId = roleCombatData.Schedule.ScheduleId,
            RoleCombatData = roleCombatData,
        };
    }
}