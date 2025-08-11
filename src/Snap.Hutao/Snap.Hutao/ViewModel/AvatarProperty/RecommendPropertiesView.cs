// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.AvatarProperty;

internal sealed class RecommendPropertiesView
{
    public ImmutableArray<string?> SandProperties { get; set; }

    public ImmutableArray<string?> GobletProperties { get; set; }

    public ImmutableArray<string?> CircletProperties { get; set; }

    public ImmutableArray<string?> SubProperties { get; set; }
}