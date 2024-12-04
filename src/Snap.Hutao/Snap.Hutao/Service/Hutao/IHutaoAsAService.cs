// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Hutao;

internal interface IHutaoAsAService
{
    ValueTask<ObservableCollection<Web.Hutao.HutaoAsAService.Announcement>> GetHutaoAnnouncementCollectionAsync(CancellationToken token = default);
}