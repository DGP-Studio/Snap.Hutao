// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Model.InterChange.Achievement;
using System.Collections.ObjectModel;
using System.Linq;
using BindingAchievement = Snap.Hutao.Model.Binding.Achievement;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;
using EntityArchive = Snap.Hutao.Model.Entity.AchievementArchive;
using MetadataAchievement = Snap.Hutao.Model.Metadata.Achievement.Achievement;

namespace Snap.Hutao.Service.Achievement;

/// <summary>
/// 成就服务
/// </summary>
[Injection(InjectAs.Transient, typeof(IAchievementService))]
internal class AchievementService : IAchievementService
{
    private readonly AppDbContext appDbContext;
    private readonly ILogger<AchievementService> logger;
    private readonly DbCurrent<EntityArchive, Message.AchievementArchiveChangedMessage> dbCurrent;
    private readonly AchievementDbOperation achievementDbOperation;

    private ObservableCollection<EntityArchive>? archiveCollection;

    /// <summary>
    /// 构造一个新的成就服务
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="logger">日志器</param>
    /// <param name="messenger">消息器</param>
    public AchievementService(AppDbContext appDbContext, ILogger<AchievementService> logger, IMessenger messenger)
    {
        this.appDbContext = appDbContext;
        this.logger = logger;

        dbCurrent = new(appDbContext, appDbContext.AchievementArchives, messenger);
        achievementDbOperation = new(appDbContext);
    }

    /// <inheritdoc/>
    public EntityArchive? CurrentArchive
    {
        get => dbCurrent.Current;
        set => dbCurrent.Current = value;
    }

    /// <inheritdoc/>
    public ObservableCollection<EntityArchive> GetArchiveCollection()
    {
        return archiveCollection ??= new(appDbContext.AchievementArchives.ToList());
    }

    /// <inheritdoc/>
    public Task RemoveArchiveAsync(EntityArchive archive)
    {
        Must.NotNull(archiveCollection!);

        // Sync cache
        archiveCollection.Remove(archive);

        // Sync database
        appDbContext.AchievementArchives.Remove(archive);
        return appDbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<ArchiveAddResult> TryAddArchiveAsync(EntityArchive newArchive)
    {
        if (string.IsNullOrEmpty(newArchive.Name))
        {
            return ArchiveAddResult.InvalidName;
        }

        Must.NotNull(archiveCollection!);

        // 查找是否有相同的名称
        if (archiveCollection.SingleOrDefault(a => a.Name == newArchive.Name) is EntityArchive userWithSameUid)
        {
            return ArchiveAddResult.AlreadyExists;
        }
        else
        {
            // Sync cache
            archiveCollection.Add(newArchive);

            // Sync database
            appDbContext.AchievementArchives.Add(newArchive);
            await appDbContext.SaveChangesAsync().ConfigureAwait(false);

            return ArchiveAddResult.Added;
        }
    }

    /// <inheritdoc/>
    public List<BindingAchievement> GetAchievements(EntityArchive archive, IList<MetadataAchievement> metadata)
    {
        Guid archiveId = archive.InnerId;
        List<EntityAchievement> entities = appDbContext.Achievements
            .Where(a => a.ArchiveId == archiveId)

            // Important! Prevent multiple sql command for SingleOrDefault below.
            .ToList();

        List<BindingAchievement> results = new();
        foreach (MetadataAchievement meta in metadata)
        {
            EntityAchievement entity = entities.SingleOrDefault(e => e.Id == meta.Id)
                ?? EntityAchievement.Create(archiveId, meta.Id);

            results.Add(new(meta, entity));
        }

        return results;
    }

    /// <inheritdoc/>
    public ImportResult ImportFromUIAF(EntityArchive archive, List<UIAFItem> list, ImportOption option)
    {
        Guid archiveId = archive.InnerId;

        switch (option)
        {
            case ImportOption.AggressiveMerge:
                {
                    IOrderedEnumerable<UIAFItem> orederedUIAF = list.OrderBy(a => a.Id);
                    return achievementDbOperation.Merge(archiveId, orederedUIAF, true);
                }

            case ImportOption.LazyMerge:
                {
                    IOrderedEnumerable<UIAFItem> orederedUIAF = list.OrderBy(a => a.Id);
                    return achievementDbOperation.Merge(archiveId, orederedUIAF, false);
                }

            case ImportOption.Overwrite:
                {
                    IEnumerable<EntityAchievement> newData = list
                        .Select(uiaf => EntityAchievement.Create(archiveId, uiaf))
                        .OrderBy(a => a.Id);
                    return achievementDbOperation.Overwrite(archiveId, newData);
                }

            default:
                throw Must.NeverHappen();
        }
    }

    /// <inheritdoc/>
    public Task<ImportResult> ImportFromUIAFAsync(EntityArchive archive, List<UIAFItem> list, ImportOption option)
    {
        return Task.Run(() => ImportFromUIAF(archive, list, option));
    }

    /// <inheritdoc/>
    public void SaveAchievements(EntityArchive archive, IList<BindingAchievement> achievements)
    {
        string name = archive.Name;
        logger.LogInformation(EventIds.Achievement, "Begin saving achievements for [{name}]", name);
        ValueStopwatch stopwatch = ValueStopwatch.StartNew();

        IEnumerable<EntityAchievement> newData = achievements
            .Where(a => a.IsChecked)
            .Select(a => a.Entity)
            .OrderBy(a => a.Id);
        ImportResult result = achievementDbOperation.Overwrite(archive.InnerId, newData);

        double time = stopwatch.GetElapsedTime().TotalMilliseconds;
        logger.LogInformation(EventIds.Achievement, "{add} added, {update} updated, {remove} removed", result.Add, result.Update, result.Remove);
        logger.LogInformation(EventIds.Achievement, "Save achievements for [{name}] completed in {time}ms", name, time);
    }
}