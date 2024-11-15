// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.SpiralAbyss;
using Snap.Hutao.ViewModel.User;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.SpiralAbyss;

internal interface ISpiralAbyssRecordService
{
    ValueTask<ObservableCollection<SpiralAbyssView>> GetSpiralAbyssViewCollectionAsync(UserAndUid userAndUid);

    ValueTask<bool> InitializeAsync();

    ValueTask RefreshSpiralAbyssAsync(UserAndUid userAndUid);
}