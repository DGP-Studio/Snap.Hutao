// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.SpiralAbyss;
using Snap.Hutao.ViewModel.User;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.SpiralAbyss;

internal interface ISpiralAbyssService
{
    ValueTask<ObservableCollection<SpiralAbyssView>> GetSpiralAbyssViewCollectionAsync(SpiralAbyssMetadataContext context, UserAndUid userAndUid);

    ValueTask RefreshSpiralAbyssAsync(SpiralAbyssMetadataContext context, UserAndUid userAndUid);
}