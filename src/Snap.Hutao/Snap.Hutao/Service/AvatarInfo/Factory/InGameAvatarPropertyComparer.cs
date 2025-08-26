// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Collection.Generic;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

internal sealed class InGameAvatarPropertyComparer : DelegatingPropertyComparer<AvatarProperty, FightProperty>
{
    private static readonly LazySlim<InGameAvatarPropertyComparer> LazyShared = new(() => new());

    private InGameAvatarPropertyComparer()
        : base(static a => a.FightProperty, InGameFightPropertyComparer.Shared)
    {
    }

    public static InGameAvatarPropertyComparer Shared { get => LazyShared.Value; }
}