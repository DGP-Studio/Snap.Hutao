// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata;

internal sealed class LevelParameters<TLevel, TParam>
{
    public LevelParameters(TLevel level, ImmutableArray<TParam> parameters)
    {
        Level = level;
        Parameters = parameters;
    }

    public TLevel Level { get; }

    public ImmutableArray<TParam> Parameters { get; }
}