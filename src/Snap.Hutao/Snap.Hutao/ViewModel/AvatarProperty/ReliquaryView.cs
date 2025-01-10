// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.AvatarProperty;

internal sealed class ReliquaryView : EquipView
{
    public ImmutableArray<ReliquaryComposedSubProperty> ComposedSubProperties { get; set; }

    public string SetName { get; set; } = default!;
}