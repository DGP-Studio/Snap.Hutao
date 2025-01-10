// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.ViewModel.Achievement;
using System.Collections.Immutable;
using EntityArchive = Snap.Hutao.Model.Entity.AchievementArchive;

namespace Snap.Hutao.Service.Achievement;

internal interface IAchievementService
{
    ValueTask<IAdvancedDbCollectionView<EntityArchive>> GetArchiveCollectionAsync(CancellationToken token = default);

    ValueTask<UIAF> ExportToUIAFAsync(EntityArchive archive);

    IAdvancedCollectionView<AchievementView> GetAchievementViewCollection(EntityArchive archive, AchievementServiceMetadataContext context);

    ValueTask<ImportResult> ImportFromUIAFAsync(EntityArchive archive, ImmutableArray<UIAFItem> array, ImportStrategyKind strategy);

    ValueTask RemoveArchiveAsync(EntityArchive archive);

    void SaveAchievement(AchievementView achievement);

    ValueTask<ArchiveAddResultKind> AddArchiveAsync(EntityArchive newArchive);
}