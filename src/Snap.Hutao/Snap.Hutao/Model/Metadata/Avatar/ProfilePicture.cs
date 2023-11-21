// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Avatar;

internal sealed class ProfilePicture
{
    public ProfilePictureId Id { get; set; }

    public string Icon { get; set; } = default!;

    public string Name { get; set; } = default!;
}