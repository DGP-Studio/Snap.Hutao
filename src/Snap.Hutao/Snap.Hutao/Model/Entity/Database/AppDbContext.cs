// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Model.Entity.Configuration;
using System.Diagnostics;

namespace Snap.Hutao.Model.Entity.Database;

[DebuggerDisplay("Id = {ContextId}")]
internal sealed class AppDbContext : DbContext
{
    private readonly ILogger<AppDbContext>? logger;

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        try
        {
            logger = this.GetService<ILogger<AppDbContext>>();
            logger?.LogColorizedInformation("{Name}[{Id}] {Action}", nameof(AppDbContext), (ContextId, ConsoleColor.DarkCyan), ("created", ConsoleColor.Green));
        }
        catch
        {
        }
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

    public DbSet<UidProfilePicture> UidProfilePictures { get; set; } = default!;

    public static AppDbContext Create(IServiceProvider serviceProvider, string sqlConnectionString)
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseApplicationServiceProvider(serviceProvider)
            .UseSqlite(sqlConnectionString)
            .Options;

        return new(options);
    }

    public override void Dispose()
    {
        base.Dispose();
        logger?.LogColorizedInformation("{Name}[{Id}] {Action}", nameof(AppDbContext), (ContextId, ConsoleColor.DarkCyan), ("disposed", ConsoleColor.Red));
    }

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