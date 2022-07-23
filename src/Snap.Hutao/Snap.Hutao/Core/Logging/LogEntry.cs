// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Core.Logging;

/// <summary>
/// 数据库日志入口点
/// </summary>
[Table("logs")]
public class LogEntry
{
    /// <summary>
    /// 内部Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    /// <summary>
    /// 类别
    /// </summary>
    public string Category { get; set; } = default!;

    /// <summary>
    /// 日志等级
    /// </summary>
    public LogLevel LogLevel { get; set; }

    /// <summary>
    /// 事件Id
    /// </summary>
    public int EventId { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; } = default!;

    /// <summary>
    /// 可能的异常
    /// </summary>
    public string? Exception { get; set; }
}