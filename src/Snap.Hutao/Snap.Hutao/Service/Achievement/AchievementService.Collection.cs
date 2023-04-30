// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Achievement;

/// <summary>
/// 集合部分
/// </summary>
internal sealed partial class AchievementService
{
    /// <inheritdoc/>
    public AchievementArchive? CurrentArchive
    {
        get => dbCurrent.Current;
        set => dbCurrent.Current = value;
    }

    /// <inheritdoc/>
    public ObservableCollection<AchievementArchive> ArchiveCollection
    {
        get
        {
            if (archiveCollection == null)
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    archiveCollection = appDbContext.AchievementArchives.ToObservableCollection();
                }

                CurrentArchive = archiveCollection.SelectedOrDefault();
            }

            return archiveCollection;
        }
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
            await taskContext.SwitchToMainThreadAsync();
            archiveCollection!.Add(newArchive);
            CurrentArchive = newArchive;

            // Sync database
            await taskContext.SwitchToBackgroundAsync();
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await appDbContext.AchievementArchives.AddAndSaveAsync(newArchive).ConfigureAwait(false);
            }

            return ArchiveAddResult.Added;
        }
    }

    /// <inheritdoc/>
    public async Task RemoveArchiveAsync(AchievementArchive archive)
    {
        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        archiveCollection!.Remove(archive);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();

        // Cascade deleted the achievements.
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.AchievementArchives
                .ExecuteDeleteWhereAsync(a => a.InnerId == archive.InnerId)
                .ConfigureAwait(false);
        }
    }
}
