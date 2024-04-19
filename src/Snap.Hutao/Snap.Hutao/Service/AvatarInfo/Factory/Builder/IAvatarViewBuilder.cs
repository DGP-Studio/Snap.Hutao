// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;

namespace Snap.Hutao.Service.AvatarInfo.Factory.Builder;

internal interface IAvatarViewBuilder : IBuilder
{
    ViewModel.AvatarProperty.AvatarView AvatarView { get; }
}