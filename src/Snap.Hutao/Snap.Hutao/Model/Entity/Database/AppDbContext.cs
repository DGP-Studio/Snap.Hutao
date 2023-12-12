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
        logger.LogInformation("{Name}[{Id}] created", nameof(AppDbContext), ContextId);
    }

    public DbSet<SettingEntry> Settings { get; set; } = default!;

    public DbSet<User> Users { get; set; } = default!;

    public DbSet<Achievement> Achievements { get; set; } = default!;

    public DbSet<AchievementArchive> AchievementArchives { get; set; } = default!;

    public DbSet<GachaItem> GachaItems { get; set; } = default!;

    public DbSet<GachaArchive> GachaArchives { get; set; } = default!;

    public DbSet<AvatarInfo> AvatarInfos { get; set; } = default!;

    public DbSet<GameAccount> GameAccounts { get; set; } = default!;

    public DbSet<DailyNoteEntry> DailyNotes { get; set; } = default!;

    public DbSet<ObjectCacheEntry> ObjectCache { get; set; } = default!;

    public DbSet<CultivateProject> CultivateProjects { get; set; } = default!;

    public DbSet<CultivateEntry> CultivateEntries { get; set; } = default!;

    public DbSet<CultivateEntryLevelInformation> LevelInformations { get; set; } = default!;

    public DbSet<CultivateItem> CultivateItems { get; set; } = default!;

    public DbSet<InventoryItem> InventoryItems { get; set; } = default!;

    public DbSet<InventoryWeapon> InventoryWeapons { get; set; } = default!;

    public DbSet<InventoryReliquary> InventoryReliquaries { get; set; } = default!;

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
        logger?.LogInformation("{Name}[{Id}] disposed", nameof(AppDbContext), ContextId);
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