// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata;

internal sealed class Combine
{
    public required CombineId Id { get; init; }

    public required uint Type { get; init; }

    public required uint SubType { get; init; }

    public required RecipeType RecipeType { get; init; }

    public required uint Cost { get; init; }

    public required IdCount Result { get; init; }

    public required ImmutableArray<IdCount> Materials { get; init; }

    public string EffectDescription { get; init; } = default!;
}