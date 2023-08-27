// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Achievement;

/// <summary>
/// 集合部分
/// </summary>
internal sealed partial class AchievementService
{
    private ObservableCollection<AchievementArchive>? archiveCollection;

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
            if (archiveCollection is null)
            {
                archiveCollection = achievementDbService.GetAchievementArchiveCollection();
                CurrentArchive = archiveCollection.SelectedOrDefault();
            }

            return archiveCollection;
        }
    }

    /// <inheritdoc/>
    public async ValueTask<ArchiveAddResult> AddArchiveAsync(AchievementArchive newArchive)
    {
        if (string.IsNullOrWhiteSpace(newArchive.Name))
        {
            return ArchiveAddResult.InvalidName;
        }

        ArgumentNullException.ThrowIfNull(archiveCollection);

        // 查找是否有相同的名称
        if (archiveCollection.Any(a => a.Name == newArchive.Name))
        {
            return ArchiveAddResult.AlreadyExists;
        }

        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        archiveCollection.Add(newArchive);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        CurrentArchive = newArchive;

        return ArchiveAddResult.Added;
    }

    /// <inheritdoc/>
    public async ValueTask RemoveArchiveAsync(AchievementArchive archive)
    {
        ArgumentNullException.ThrowIfNull(archiveCollection);

        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        archiveCollection.Remove(archive);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        await achievementDbService.DeleteAchievementArchiveAsync(archive).ConfigureAwait(false);
    }
}
