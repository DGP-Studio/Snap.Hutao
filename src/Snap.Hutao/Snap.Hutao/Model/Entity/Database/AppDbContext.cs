// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model.Entity.Configuration;
using System.Diagnostics;

namespace Snap.Hutao.Model.Entity.Database;

/// <summary>
/// 应用程序数据库上下文
/// </summary>
[HighQuality]
[DebuggerDisplay("Id = {ContextId}")]
internal sealed class AppDbContext : DbContext
{
    private readonly ILogger<AppDbContext>? logger;

    /// <summary>
    /// 构造一个新的应用程序数据库上下文
    /// </summary>
    /// <param name="options">选项</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// 构造一个新的应用程序数据库上下文
    /// </summary>
    /// <param name="options">选项</param>
    /// <param name="logger">日志器</param>
    public AppDbContext(DbContextOptions<AppDbContext> options, ILogger<AppDbContext> logger)
        : this(options)
    {
        this.logger = logger;
        logger.LogInformation("{name}[{id}] created.", nameof(AppDbContext), ContextId);
    }

    /// <summary>
    /// 设置
    /// </summary>
    public DbSet<SettingEntry> Settings { get; set; } = default!;

    /// <summary>
    /// 用户
    /// </summary>
    public DbSet<User> Users { get; set; } = default!;

    /// <summary>
    /// 成就
    /// </summary>
    public DbSet<Achievement> Achievements { get; set; } = default!;

    /// <summary>
    /// 成就存档
    /// </summary>
    public DbSet<AchievementArchive> AchievementArchives { get; set; } = default!;

    /// <summary>
    /// 卡池数据
    /// </summary>
    public DbSet<GachaItem> GachaItems { get; set; } = default!;

    /// <summary>
    /// 卡池存档
    /// </summary>
    public DbSet<GachaArchive> GachaArchives { get; set; } = default!;

    /// <summary>
    /// 角色信息
    /// </summary>
    public DbSet<AvatarInfo> AvatarInfos { get; set; } = default!;

    /// <summary>
    /// 游戏内账号
    /// </summary>
    public DbSet<GameAccount> GameAccounts { get; set; } = default!;

    /// <summary>
    /// 实时便笺
    /// </summary>
    public DbSet<DailyNoteEntry> DailyNotes { get; set; } = default!;

    /// <summary>
    /// 对象缓存
    /// </summary>
    public DbSet<ObjectCacheEntry> ObjectCache { get; set; } = default!;

    /// <summary>
    /// 培养计划
    /// </summary>
    public DbSet<CultivateProject> CultivateProjects { get; set; } = default!;

    /// <summary>
    /// 培养入口点
    /// </summary>
    public DbSet<CultivateEntry> CultivateEntries { get; set; } = default!;

    /// <summary>
    /// 培养消耗物品
    /// </summary>
    public DbSet<CultivateItem> CultivateItems { get; set; } = default!;

    /// <summary>
    /// 背包内物品
    /// </summary>
    public DbSet<InventoryItem> InventoryItems { get; set; } = default!;

    /// <summary>
    /// 背包内武器
    /// </summary>
    public DbSet<InventoryWeapon> InventoryWeapons { get; set; } = default!;

    /// <summary>
    /// 背包内圣遗物
    /// </summary>
    public DbSet<InventoryReliquary> InventoryReliquaries { get; set; } = default!;

    /// <summary>
    /// 深渊记录
    /// </summary>
    public DbSet<SpiralAbyssEntry> SpiralAbysses { get; set; } = default!;

    /// <summary>
    /// 构造一个临时的应用程序数据库上下文
    /// </summary>
    /// <param name="sqlConnectionString">连接字符串</param>
    /// <returns>应用程序数据库上下文</returns>
    public static AppDbContext Create(string sqlConnectionString)
    {
        return new(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(sqlConnectionString).Options);
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        base.Dispose();
        logger?.LogInformation("AppDbContext[{id}] disposed.", ContextId);
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfiguration(new AvatarInfoConfiguration())
            .ApplyConfiguration(new DailyNoteEntryConfiguration())
            .ApplyConfiguration(new InventoryReliquaryConfiguration())
            .ApplyConfiguration(new SpiralAbyssEntryConfiguration())
            .ApplyConfiguration(new UserConfiguration());
    }
}