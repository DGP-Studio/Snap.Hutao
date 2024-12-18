// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.ViewModel.AvatarProperty;

internal class ReliquaryComposedSubProperty : ReliquarySubProperty
{
    public ReliquaryComposedSubProperty(FightProperty type, string value, uint enhancedCount)
        : base(type, value)
    {
        Type = type;
        EnhancedCount = enhancedCount;
    }

    public uint EnhancedCount { get; set; }

    internal FightProperty Type { get; }
}