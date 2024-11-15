// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.ViewModel.AvatarProperty;

internal sealed class Summary
{
    public AdvancedCollectionView<AvatarView> Avatars { get; set; } = default!;

    public string Message { get; set; } = default!;
}