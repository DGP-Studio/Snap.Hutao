// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Logging;

namespace Snap.Hutao.Context.Database;

/// <summary>
/// 日志数据库上下文
/// 由于写入日志的行为需要锁定数据库上下文
/// 所以将日志单独分离出来进行读写
/// </summary>
public class LogDbContext : DbContext
{
    /// <summary>
    /// 创建一个新的
    /// </summary>
    /// <param name="options">选项</param>
    private LogDbContext(DbContextOptions<LogDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// 日志记录
    /// </summary>
    public DbSet<LogEntry> Logs { get; set; } = default!;

    /// <summary>
    /// 构造一个临时的日志数据库上下文
    /// </summary>
    /// <param name="sqlConnectionString">连接字符串</param>
    /// <returns>日志数据库上下文</returns>
    public static LogDbContext Create(string sqlConnectionString)
    {
        return new(new DbContextOptionsBuilder<LogDbContext>().UseSqlite(sqlConnectionString).Options);
    }
}