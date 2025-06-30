// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.ViewModel.AvatarProperty;

internal sealed class Summary
{
    public IAdvancedCollectionView<AvatarView> Avatars { get; set; } = default!;

    public static Summary Create(IAdvancedCollectionView<AvatarView> avatars)
    {
        return new()
        {
            Avatars = avatars,
        };
    }
}