// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Reliquary;

internal sealed class ReliquaryMainAffixLevel
{
    public required QualityType Rank { get; init; }

    public required uint Level { get; init; }

    public required TypeValueCollection<FightProperty, float> Properties { get; init; }
}