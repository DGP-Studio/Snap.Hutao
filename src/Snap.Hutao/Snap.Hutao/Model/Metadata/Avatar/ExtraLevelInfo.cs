// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata.Avatar;

internal sealed class ExtraLevelInfo
{
    public required ExtraLevelIndexKind Index { get; init; }

    public required uint Level { get; init; }
}