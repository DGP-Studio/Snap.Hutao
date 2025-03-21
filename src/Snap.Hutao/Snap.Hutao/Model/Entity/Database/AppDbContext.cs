// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Snap.Hutao.Model.Entity.Configuration;
using System.Diagnostics;

namespace Snap.Hutao.Model.Entity.Database;

[DebuggerDisplay("Id = {ContextId}")]
internal sealed partial class AppDbContext : DbContext
{
    private readonly ILogger<AppDbContext>? logger;

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        try
        {
            IServiceProviderIsService serviceProviderIsService = this.GetService<IServiceProviderIsService>();
            if (serviceProviderIsService.IsService(typeof(ILogger<AppDbContext>)))
            {
                logger = this.GetService<ILogger<AppDbContext>>();
                logger.LogInformation("\e[1m\e[32m{Name}\e[37m::\e[36m{ContextId} \e[32mcreated\e[37m", nameof(AppDbContext), ContextId);
            }
        }
        catch
        {
            // ignored
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

    public DbSet<CultivateEntryLevelInformation> LevelInformation { get; set; } = default!;

    public DbSet<CultivateItem> CultivateItems { get; set; } = default!;

    public DbSet<InventoryItem> InventoryItems { get; set; } = default!;

    public DbSet<SpiralAbyssEntry> SpiralAbysses { get; set; } = default!;

    public DbSet<UidProfilePicture> UidProfilePictures { get; set; } = default!;

    public DbSet<RoleCombatEntry> RoleCombats { get; set; } = default!;

    public DbSet<AvatarStrategy> AvatarStrategies { get; set; } = default!;

    public static AppDbContext Create(IServiceProvider serviceProvider, string sqlConnectionString)
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(sqlConnectionString)
            .Options;

        return new(options);
    }

    public override void Dispose()
    {
        base.Dispose();
        logger?.LogInformation("\e[1m\e[32m{Name}\e[37m::\e[36m{ContextId} \e[31mdisposed\e[37m", nameof(AppDbContext), ContextId);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfiguration(new AvatarInfoConfiguration())
            .ApplyConfiguration(new DailyNoteEntryConfiguration())
            .ApplyConfiguration(new SpiralAbyssEntryConfiguration())
            .ApplyConfiguration(new RoleCombatEntryConfiguration())
            .ApplyConfiguration(new UserConfiguration());
    }
}