// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.AvatarProperty;

internal sealed class RecommendPropertiesView
{
    public required ImmutableArray<string> SandProperties { get; init; }

    public required ImmutableArray<string> GobletProperties { get; init; }

    public required ImmutableArray<string> CircletProperties { get; init; }

    public required ImmutableArray<string> SubProperties { get; init; }
}