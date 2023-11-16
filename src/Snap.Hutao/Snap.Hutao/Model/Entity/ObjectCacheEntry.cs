// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 数据库对象缓存
/// </summary>
[HighQuality]
[Table("object_cache")]
internal sealed class ObjectCacheEntry
{
    /// <summary>
    /// 主键
    /// </summary>
    [Key]
    public string Key { get; set; } = default!;

    /// <summary>
    /// 过期时间
    /// </summary>
    public DateTimeOffset ExpireTime { get; set; }

    /// <summary>
    /// 获取该对象是否过期
    /// </summary>
    [NotMapped]
    public bool IsExpired { get => ExpireTime < DateTimeOffset.UtcNow; }

    /// <summary>
    /// 值字符串
    /// </summary>
    public string? Value { get; set; }
}