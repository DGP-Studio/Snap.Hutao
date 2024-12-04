// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;

namespace Snap.Hutao.ViewModel.AvatarProperty;

internal abstract class NameIconDescription : NameDescription, INameIcon<Uri>
{
    public Uri Icon { get; set; } = default!;
}