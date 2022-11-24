// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Model.Intrinsic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 成就
/// </summary>
[Table("achievements")]
public class Achievement : IEquatable<Achievement>
{
    /// <summary>
    /// 内部Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    /// <summary>
    /// 存档
    /// </summary>
    [ForeignKey(nameof(ArchiveId))]
    public AchievementArchive Archive { get; set; } = default!;

    /// <summary>
    /// 存档Id
    /// </summary>
    public Guid ArchiveId { get; set; }

    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 当前进度
    /// </summary>
    public int Current { get; set; }

    /// <summary>
    /// 完成时间
    /// </summary>
    public DateTimeOffset Time { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public AchievementInfoStatus Status { get; set; }

    /// <summary>
    /// 创建一个新的成就
    /// </summary>
    /// <param name="userId">对应的用户id</param>
    /// <param name="id">成就Id</param>
    /// <returns>新创建的成就</returns>
    public static Achievement Create(Guid userId, int id)
    {
        return new()
        {
            ArchiveId = userId,
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
    public static Achievement Create(Guid userId, UIAFItem uiaf)
    {
        return new()
        {
            ArchiveId = userId,
            Id = uiaf.Id,
            Current = uiaf.Current,
            Status = uiaf.Status, // Hot fix | 1.0.30 | Status not set when create database entity
            Time = DateTimeOffset.FromUnixTimeSeconds(uiaf.Timestamp).ToLocalTime(),
        };
    }

    /// <summary>
    /// 转换到UIAF物品
    /// </summary>
    /// <returns>UIAF物品</returns>
    public UIAFItem ToUIAFItem()
    {
        return new()
        {
            Id = Id,
            Current = Current,
            Status = Status,
            Timestamp = Time.ToUniversalTime().ToUnixTimeSeconds(),
        };
    }

    /// <inheritdoc/>
    public bool Equals(Achievement? other)
    {
        if (other == null)
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
}