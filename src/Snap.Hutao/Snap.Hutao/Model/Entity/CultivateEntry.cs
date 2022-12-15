// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Cultivation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 养成入口
/// </summary>
[Table("cultivate_entries")]
public class CultivateEntry
{
    /// <summary>
    /// 内部Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    /// <summary>
    /// 项目Id
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// 项目
    /// </summary>
    [ForeignKey(nameof(ProjectId))]
    public CultivateProject Project { get; set; } = default!;

    /// <summary>
    /// 养成类型
    /// </summary>
    public CultivateType Type { get; set; }

    /// <summary>
    /// 角色/武器/家具 Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 创建一个新的养成入口点
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="id">主Id</param>
    /// <returns>养成入口点</returns>
    public static CultivateEntry Create(Guid projectId,CultivateType type, int id)
    {
        return new()
        {
            ProjectId = projectId,
            Type = type,
            Id = id,
        };
    }
}