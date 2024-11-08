// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;

namespace Snap.Hutao.ViewModel.RoleCombat;

internal sealed class AvatarDamage : AvatarView
{
    public AvatarDamage(string value, Avatar metaAvatar)
        : base(metaAvatar)
    {
        Value = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }

    public int Value { get; }
}