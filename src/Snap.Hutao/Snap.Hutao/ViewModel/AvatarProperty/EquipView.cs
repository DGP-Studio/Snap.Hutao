// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.ViewModel.AvatarProperty;

internal abstract class EquipView : NameIconDescription
{
    public string Level { get; set; } = default!;

    public QualityType Quality { get; set; }

    public NameValue<string>? MainProperty { get; set; }

    internal EquipType EquipType { get; set; }
}
