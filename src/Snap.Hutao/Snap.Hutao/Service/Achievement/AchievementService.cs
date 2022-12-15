// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Model.InterChange.Achievement;
using System.Collections.ObjectModel;
using BindingAchievement = Snap.Hutao.Model.Binding.Achievement;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;
using EntityArchive = Snap.Hutao.Model.Entity.AchievementArchive;
using MetadataAchievement = Snap.Hutao.Model.Metadata.Achievement.Achievement;

namespace Snap.Hutao.Service.Achievement;

/// <summary>
/// 成就服务
/// </summary>
[Injection(InjectAs.Scoped, typeof(IAchievementService))]
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

        dbCurrent = new(appDbContext.AchievementArchives, messenger);
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
        return archiveCollection ??= new(appDbContext.AchievementArchives.AsNoTracking().ToList());
    }

    /// <inheritdoc/>
    public async Task RemoveArchiveAsync(EntityArchive archive)
    {
        // Sync cache
        // Keep this on main thread.
        await ThreadHelper.SwitchToMainThreadAsync();
        archiveCollection!.Remove(archive);

        // Sync database
        await ThreadHelper.SwitchToBackgroundAsync();
        await appDbContext.AchievementArchives.RemoveAndSaveAsync(archive).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<ArchiveAddResult> TryAddArchiveAsync(EntityArchive newArchive)
    {
        if (string.IsNullOrWhiteSpace(newArchive.Name))
        {
            return ArchiveAddResult.InvalidName;
        }

        // 查找是否有相同的名称
        if (archiveCollection!.SingleOrDefault(a => a.Name == newArchive.Name) != null)
        {
            return ArchiveAddResult.AlreadyExists;
        }
        else
        {
            // Sync cache
            await ThreadHelper.SwitchToMainThreadAsync();
            archiveCollection!.Add(newArchive);

            // Sync database
            await ThreadHelper.SwitchToBackgroundAsync();
            await appDbContext.AchievementArchives.AddAndSaveAsync(newArchive).ConfigureAwait(false);

            return ArchiveAddResult.Added;
        }
    }

    /// <inheritdoc/>
    public List<BindingAchievement> GetAchievements(EntityArchive archive, IList<MetadataAchievement> metadata)
    {
        Guid archiveId = archive.InnerId;
        List<EntityAchievement> entities = appDbContext.Achievements
            .Where(a => a.ArchiveId == archiveId)
            .ToList();

        List<BindingAchievement> results = new();
        foreach (MetadataAchievement meta in metadata)
        {
            EntityAchievement entity = entities.SingleOrDefault(e => e.Id == meta.Id) ?? EntityAchievement.Create(archiveId, meta.Id);

            results.Add(new(meta, entity));
        }

        return results;
    }

    /// <inheritdoc/>
    public async Task<UIAF> ExportToUIAFAsync(EntityArchive archive)
    {
        await ThreadHelper.SwitchToBackgroundAsync();
        List<UIAFItem> list = appDbContext.Achievements
            .Where(i => i.ArchiveId == archive.InnerId)
            .AsEnumerable()
            .Select(i => i.ToUIAFItem())
            .ToList();

        UIAF uigf = new()
        {
            Info = UIAFInfo.Create(),
            List = list,
        };

        return uigf;
    }

    /// <inheritdoc/>
    public async Task<ImportResult> ImportFromUIAFAsync(EntityArchive archive, List<UIAFItem> list, ImportStrategy strategy)
    {
        // return Task.Run(() => ImportFromUIAF(archive, list, strategy));
        await ThreadHelper.SwitchToBackgroundAsync();

        Guid archiveId = archive.InnerId;

        switch (strategy)
        {
            case ImportStrategy.AggressiveMerge:
                {
                    IOrderedEnumerable<UIAFItem> orederedUIAF = list.OrderBy(a => a.Id);
                    return achievementDbOperation.Merge(archiveId, orederedUIAF, true);
                }

            case ImportStrategy.LazyMerge:
                {
                    IOrderedEnumerable<UIAFItem> orederedUIAF = list.OrderBy(a => a.Id);
                    return achievementDbOperation.Merge(archiveId, orederedUIAF, false);
                }

            case ImportStrategy.Overwrite:
                {
                    IEnumerable<EntityAchievement> orederedUIAF = list
                        .Select(uiaf => EntityAchievement.Create(archiveId, uiaf))
                        .OrderBy(a => a.Id);
                    return achievementDbOperation.Overwrite(archiveId, orederedUIAF);
                }

            default:
                throw Must.NeverHappen();
        }
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