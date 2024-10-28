// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.Cultivation;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Cultivation;

internal static class CultivationServiceExtension
{
    public static ValueTask<ObservableCollection<CultivateEntryView>?> GetCultivateEntryCollectionForCurrentProjectAsync(this ICultivationService cultivationService, ICultivationMetadataContext context)
    {
        if (cultivationService.Projects.CurrentItem is not { } project)
        {
            return default;
        }

#pragma warning disable CS8619
        return cultivationService.GetCultivateEntryCollectionAsync(project, context);
#pragma warning restore CS8619
    }
}