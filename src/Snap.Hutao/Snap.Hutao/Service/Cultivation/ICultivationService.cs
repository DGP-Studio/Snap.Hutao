// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Cultivation.Consumption;
using Snap.Hutao.ViewModel.Cultivation;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Cultivation;

internal interface ICultivationService
{
    AdvancedDbCollectionView<CultivateProject> Projects { get; }

    ValueTask<bool> EnsureCurrentProjectAsync();

    ValueTask<ObservableCollection<CultivateEntryView>> GetCultivateEntryCollectionAsync(CultivateProject cultivateProject, ICultivationMetadataContext context);

    ValueTask<ObservableCollection<StatisticsCultivateItem>> GetStatisticsCultivateItemCollectionAsync(
        CultivateProject cultivateProject, ICultivationMetadataContext context, CancellationToken token);

    ValueTask RemoveCultivateEntryAsync(Guid entryId);

    ValueTask RemoveProjectAsync(CultivateProject project);

    ValueTask<ConsumptionSaveResultKind> SaveConsumptionAsync(InputConsumption inputConsumption);

    void SaveCultivateItem(CultivateItemView item);

    ValueTask<ProjectAddResultKind> TryAddProjectAsync(CultivateProject project);
}
