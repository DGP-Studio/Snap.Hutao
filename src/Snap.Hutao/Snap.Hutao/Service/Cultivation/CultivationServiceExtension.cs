// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.ViewModel.Cultivation;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Cultivation;

internal static class CultivationServiceExtension
{
    public static async ValueTask<ObservableCollection<CultivateEntryView>?> GetCultivateEntryCollectionForCurrentProjectAsync(this ICultivationService cultivationService, ICultivationMetadataContext context)
    {
        if (!await cultivationService.EnsureCurrentProjectAsync().ConfigureAwait(false))
        {
            return default;
        }

        ArgumentNullException.ThrowIfNull(cultivationService.Projects.CurrentItem);
        return await cultivationService.GetCultivateEntryCollectionAsync(cultivationService.Projects.CurrentItem, context).ConfigureAwait(false);
    }
}