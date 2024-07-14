// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.AvatarProperty;

internal sealed class ReliquaryView : EquipView
{
    public List<ReliquarySubProperty> PrimarySubProperties { get; set; } = default!;

    public List<ReliquarySubProperty> SecondarySubProperties { get; set; } = default!;

    public List<ReliquaryComposedSubProperty> ComposedSubProperties { get; set; } = default!;

    public string ScoreFormatted { get => $"{Score:F2}"; }

    internal float Score { get; set; }

    internal string SetName { get; set; } = default!;
}