// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.AvatarProperty;

internal sealed class ReliquaryView : EquipView
{
    public List<ReliquaryComposedSubProperty> ComposedSubProperties { get; set; } = default!;

    public string SetName { get; set; } = default!;
}