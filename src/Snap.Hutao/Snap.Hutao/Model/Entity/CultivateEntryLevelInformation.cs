// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service.Cultivation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

[Table("cultivate_entry_level_informations")]
internal sealed class CultivateEntryLevelInformation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    public Guid EntryId { get; set; }

    [ForeignKey(nameof(EntryId))]
    public CultivateEntry? Entry { get; set; }

    public uint AvatarLevelFrom { get; set; }

    public uint AvatarLevelTo { get; set; }

    public bool AvatarIsPromoting { get; set; }

    public uint SkillALevelFrom { get; set; }

    public uint SkillALevelTo { get; set; }

    public uint SkillELevelFrom { get; set; }

    public uint SkillELevelTo { get; set; }

    public uint SkillQLevelFrom { get; set; }

    public uint SkillQLevelTo { get; set; }

    public uint WeaponLevelFrom { get; set; }

    public uint WeaponLevelTo { get; set; }

    public bool WeaponIsPromoting { get; set; }

    public static CultivateEntryLevelInformation From(Guid entryId, CultivateType type, LevelInformation source)
    {
        return type switch
        {
            CultivateType.AvatarAndSkill => new()
            {
                EntryId = entryId,
                AvatarLevelFrom = source.AvatarLevelFrom,
                AvatarLevelTo = source.AvatarLevelTo,
                AvatarIsPromoting = source.AvatarIsPromoting,
                SkillALevelFrom = source.SkillALevelFrom,
                SkillALevelTo = source.SkillALevelTo,
                SkillELevelFrom = source.SkillELevelFrom,
                SkillELevelTo = source.SkillELevelTo,
                SkillQLevelFrom = source.SkillQLevelFrom,
                SkillQLevelTo = source.SkillQLevelTo,
            },
            CultivateType.Weapon => new()
            {
                EntryId = entryId,
                WeaponLevelFrom = source.WeaponLevelFrom,
                WeaponLevelTo = source.WeaponLevelTo,
                WeaponIsPromoting = source.WeaponIsPromoting,
            },
            _ => throw HutaoException.NotSupported($"不支持的养成类型{type}"),
        };
    }
}