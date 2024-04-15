// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity.Abstraction;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 成就
/// </summary>
[HighQuality]
[Table("achievements")]
[SuppressMessage("", "SA1124")]
internal sealed class Achievement : IAppDbEntity,
    IEquatable<Achievement>,
    IDbMappingForeignKeyFrom<Achievement, AchievementId>,
    IDbMappingForeignKeyFrom<Achievement, UIAFItem>
{
    /// <summary>
    /// 内部Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    /// <summary>
    /// 存档 Id
    /// </summary>
    public Guid ArchiveId { get; set; }

    /// <summary>
    /// 存档
    /// </summary>
    [ForeignKey(nameof(ArchiveId))]
    public AchievementArchive Archive { get; set; } = default!;

    /// <summary>
    /// Id
    /// </summary>
    public uint Id { get; set; }

    /// <summary>
    /// 当前进度
    /// </summary>
    public uint Current { get; set; }

    /// <summary>
    /// 完成时间
    /// </summary>
    public DateTimeOffset Time { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public AchievementStatus Status { get; set; }

    /// <summary>
    /// 创建一个新的成就
    /// </summary>
    /// <param name="archiveId">对应的用户id</param>
    /// <param name="id">成就Id</param>
    /// <returns>新创建的成就</returns>
    public static Achievement From(in Guid archiveId, in AchievementId id)
    {
        return new()
        {
            ArchiveId = archiveId,
            Id = id,
            Current = 0,
            Time = DateTimeOffset.MinValue,
        };
    }

    /// <summary>
    /// 创建一个新的成就
    /// </summary>
    /// <param name="userId">对应的用户id</param>
    /// <param name="uiaf">uiaf项</param>
    /// <returns>新创建的成就</returns>
    public static Achievement From(in Guid userId, in UIAFItem uiaf)
    {
        return new()
        {
            ArchiveId = userId,
            Id = uiaf.Id,
            Current = uiaf.Current,
            Status = uiaf.Status,
            Time = DateTimeOffset.FromUnixTimeSeconds(uiaf.Timestamp).ToLocalTime(),
        };
    }

    /// <inheritdoc/>
    public bool Equals(Achievement? other)
    {
        if (other is null)
        {
            return false;
        }
        else
        {
            return ArchiveId == other.ArchiveId
                && Id == other.Id
                && Current == other.Current
                && Status == other.Status
                && Time == other.Time;
        }
    }

    #region Object

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Achievement);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(ArchiveId, Id, Current, Status, Time);
    }
    #endregion
}