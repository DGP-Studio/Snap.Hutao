// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.Achievement;
using System.Collections.ObjectModel;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;
using MetadataAchievement = Snap.Hutao.Model.Metadata.Achievement.Achievement;

namespace Snap.Hutao.Service.Achievement;

/// <summary>
/// 成就服务
/// </summary>
[HighQuality]
[Injection(InjectAs.Scoped, typeof(IAchievementService))]
internal sealed class AchievementService : IAchievementService
{
    private readonly AppDbContext appDbContext;
    private readonly ILogger<AchievementService> logger;
    private readonly DbCurrent<AchievementArchive, Message.AchievementArchiveChangedMessage> dbCurrent;
    private readonly AchievementDbOperation achievementDbOperation;
    private readonly IServiceProvider serviceProvider;

    private ObservableCollection<AchievementArchive>? archiveCollection;

    /// <summary>
    /// 构造一个新的成就服务
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="logger">日志器</param>
    /// <param name="messenger">消息器</param>
    public AchievementService(IServiceProvider serviceProvider, AppDbContext appDbContext, ILogger<AchievementService> logger, IMessenger messenger)
    {
        this.appDbContext = appDbContext;
        this.logger = logger;
        this.serviceProvider = serviceProvider;

        dbCurrent = new(appDbContext.AchievementArchives, messenger);
        achievementDbOperation = new(appDbContext);
    }

    /// <inheritdoc/>
    public AchievementArchive? CurrentArchive
    {
        get => dbCurrent.Current;
        set => dbCurrent.Current = value;
    }

    /// <inheritdoc/>
    public async Task<ObservableCollection<AchievementArchive>> GetArchiveCollectionAsync()
    {
        await ThreadHelper.SwitchToMainThreadAsync();
        return archiveCollection ??= appDbContext.AchievementArchives.AsNoTracking().ToObservableCollection();
    }

    /// <inheritdoc/>
    public async Task RemoveArchiveAsync(AchievementArchive archive)
    {
        // Sync cache
        await ThreadHelper.SwitchToMainThreadAsync();
        archiveCollection!.Remove(archive);

        // Sync database
        await ThreadHelper.SwitchToBackgroundAsync();

        // Cascade deleted the achievements.
        await appDbContext.AchievementArchives
            .ExecuteDeleteWhereAsync(a => a.InnerId == archive.InnerId)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<ArchiveAddResult> TryAddArchiveAsync(AchievementArchive newArchive)
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
    public List<AchievementView> GetAchievements(AchievementArchive archive, List<MetadataAchievement> metadata)
    {
        Dictionary<int, EntityAchievement> entityMap;
        try
        {
            entityMap = appDbContext.Achievements
                .Where(a => a.ArchiveId == archive.InnerId)
                .AsEnumerable()
                .ToDictionary(a => a.Id);
        }
        catch (ArgumentException ex)
        {
            throw ThrowHelper.UserdataCorrupted(SH.ServiceAchievementUserdataCorruptedInnerIdNotUnique, ex);
        }

        List<AchievementView> results = new();
        foreach (MetadataAchievement meta in metadata)
        {
            EntityAchievement? entity = entityMap.GetValueOrDefault(meta.Id) ?? EntityAchievement.Create(archive.InnerId, meta.Id);
            results.Add(new(entity, meta));
        }

        return results;
    }

    /// <inheritdoc/>
    public async Task<List<AchievementStatistics>> GetAchievementStatisticsAsync(Dictionary<AchievementId, MetadataAchievement> achievementMap)
    {
        await ThreadHelper.SwitchToBackgroundAsync();

        List<AchievementStatistics> results = new();
        foreach (AchievementArchive archive in appDbContext.AchievementArchives)
        {
            int finished = await appDbContext.Achievements
                .Where(a => a.ArchiveId == archive.InnerId)
                .Where(a => (int)a.Status >= (int)Model.Intrinsic.AchievementStatus.STATUS_FINISHED)
                .CountAsync()
                .ConfigureAwait(false);
            int count = achievementMap.Count;

            List<EntityAchievement> achievements = await appDbContext.Achievements
                .Where(a => a.ArchiveId == archive.InnerId)
                .OrderByDescending(a => a.Time.ToString())
                .Take(2)
                .ToListAsync()
                .ConfigureAwait(false);

            results.Add(new()
            {
                DisplayName = archive.Name,
                FinishDescription = $"{finished}/{count} - {(double)finished / count:P2}",
                Achievements = achievements.SelectList(entity => new AchievementView(entity, achievementMap[entity.Id])),
            });
        }

        return results;
    }

    /// <inheritdoc/>
    public async Task<UIAF> ExportToUIAFAsync(AchievementArchive archive)
    {
        await ThreadHelper.SwitchToBackgroundAsync();
        List<UIAFItem> list = appDbContext.Achievements
            .Where(i => i.ArchiveId == archive.InnerId)
            .AsEnumerable()
            .Select(i => i.ToUIAFItem())
            .ToList();

        UIAF uigf = new()
        {
            Info = UIAFInfo.Create(serviceProvider),
            List = list,
        };

        return uigf;
    }

    /// <inheritdoc/>
    public async Task<ImportResult> ImportFromUIAFAsync(AchievementArchive archive, List<UIAFItem> list, ImportStrategy strategy)
    {
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
    public void SaveAchievements(AchievementArchive archive, List<AchievementView> achievements)
    {
        string name = archive.Name;
        logger.LogInformation("Begin saving achievements for [{name}]", name);
        ValueStopwatch stopwatch = ValueStopwatch.StartNew();

        IEnumerable<EntityAchievement> newData = achievements
            .Where(a => a.IsChecked)
            .Select(a => a.Entity)
            .OrderBy(a => a.Id);
        ImportResult result = achievementDbOperation.Overwrite(archive.InnerId, newData);

        double time = stopwatch.GetElapsedTime().TotalMilliseconds;
        logger.LogInformation("{add} added, {update} updated, {remove} removed", result.Add, result.Update, result.Remove);
        logger.LogInformation("Save achievements for [{name}] completed in {time}ms", name, time);
    }

    /// <inheritdoc/>
    public void SaveAchievement(AchievementView achievement)
    {
        // Delete exists one.
        appDbContext.Achievements.ExecuteDeleteWhere(e => e.InnerId == achievement.Entity.InnerId);
        if (achievement.IsChecked)
        {
            // treat as new created.
            achievement.Entity.InnerId = default;
            appDbContext.Achievements.AddAndSave(achievement.Entity);
        }
    }
}