// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

[Table("role_combats")]
internal sealed partial class RoleCombatEntry : ObservableObject
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    public uint ScheduleId { get; set; }

    public string Uid { get; set; } = default!;

    public RoleCombatData RoleCombatData { get; set; } = default!;

    public static RoleCombatEntry From(string uid, RoleCombatData roleCombatData)
    {
        return new()
        {
            Uid = uid,
            ScheduleId = roleCombatData.Schedule.ScheduleId,
            RoleCombatData = roleCombatData,
        };
    }
}