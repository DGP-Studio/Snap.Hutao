// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Context.Database;

/// <summary>
/// 应用程序数据库上下文
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// 构造一个新的应用程序数据库上下文
    /// </summary>
    /// <param name="options">选项</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// 设置项
    /// </summary>
    public DbSet<SettingEntry> Settings { get; set; } = default!;

    /// <summary>
    /// 用户表
    /// </summary>
    public DbSet<User> Users { get; set; } = default!;

    /// <summary>
    /// 构造一个临时的应用程序数据库上下文
    /// </summary>
    /// <param name="sqlConnectionString">连接字符串</param>
    /// <returns>应用程序数据库上下文</returns>
    public static AppDbContext Create(string sqlConnectionString)
    {
        return new(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(sqlConnectionString).Options);
    }
}