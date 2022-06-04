// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Context.Database;

/// <summary>
/// 应用程序数据库上下文
/// </summary>
internal class AppDbContext : DbContext
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
}
