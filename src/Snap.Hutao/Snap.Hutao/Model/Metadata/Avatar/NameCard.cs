// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata.Avatar;

internal sealed class NameCard : NameDescription
{
    public required string Icon { get; init; }

    public required string PicturePrefix { get; init; }
}