// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.ViewModel.Achievement;
using EntityArchive = Snap.Hutao.Model.Entity.AchievementArchive;

namespace Snap.Hutao.Service.Achievement;

internal interface IAchievementService
{
    AdvancedDbCollectionView<EntityArchive> Archives { get; }

    ValueTask<UIAF> ExportToUIAFAsync(EntityArchive selectedArchive);

    List<AchievementView> GetAchievementViewList(EntityArchive archive, AchievementServiceMetadataContext context);

    ValueTask<ImportResult> ImportFromUIAFAsync(EntityArchive archive, List<UIAFItem> list, ImportStrategyKind strategy);

    ValueTask RemoveArchiveAsync(EntityArchive archive);

    void SaveAchievement(AchievementView achievement);

    ValueTask<ArchiveAddResultKind> AddArchiveAsync(EntityArchive newArchive);
}