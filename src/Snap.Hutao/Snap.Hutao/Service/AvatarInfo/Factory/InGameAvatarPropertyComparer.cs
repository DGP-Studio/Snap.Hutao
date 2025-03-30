// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

internal sealed class InGameAvatarPropertyComparer : IComparer<AvatarProperty>
{
    private static readonly LazySlim<InGameAvatarPropertyComparer> LazyShared = new(() => new());

    private readonly InGameFightPropertyComparer comparer = InGameFightPropertyComparer.Shared;

    public static InGameAvatarPropertyComparer Shared { get => LazyShared.Value; }

    public int Compare(AvatarProperty? x, AvatarProperty? y)
    {
        return (x, y) switch
        {
            (null, not null) => -1,
            (not null, null) => 1,
            (null, null) => 0,
            (not null, not null) => comparer.Compare(x.FightProperty, y.FightProperty),
        };
    }
}