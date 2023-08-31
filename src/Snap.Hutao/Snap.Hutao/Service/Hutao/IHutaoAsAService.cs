// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hutao.HutaoAsAService;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Hutao;

internal interface IHutaoAsAService
{
    ValueTask<ObservableCollection<Announcement>> GetHutaoAnnouncementCollectionAsync(CancellationToken token = default);
}