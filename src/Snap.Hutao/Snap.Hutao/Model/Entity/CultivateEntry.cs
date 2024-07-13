// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity.Abstraction;
using Snap.Hutao.Model.Entity.Primitive;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

[Table("cultivate_entries")]
internal sealed class CultivateEntry : IAppDbEntity,
    IDbMappingForeignKeyFrom<CultivateEntry, CultivateType, uint>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    public Guid ProjectId { get; set; }

    [ForeignKey(nameof(ProjectId))]
    public CultivateProject Project { get; set; } = default!;

    public CultivateEntryLevelInformation? LevelInformation { get; set; }

    public CultivateType Type { get; set; }

    public uint Id { get; set; }

    public static CultivateEntry From(in Guid projectId, in CultivateType type, in uint id)
    {
        return new()
        {
            ProjectId = projectId,
            Type = type,
            Id = id,
        };
    }
}