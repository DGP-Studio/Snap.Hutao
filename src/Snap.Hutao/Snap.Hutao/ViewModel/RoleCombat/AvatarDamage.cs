// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using System.Globalization;

namespace Snap.Hutao.ViewModel.RoleCombat;

internal sealed class AvatarDamage : AvatarView
{
    public AvatarDamage(string value, Avatar metaAvatar)
        : base(metaAvatar)
    {
        int.TryParse(value, CultureInfo.InvariantCulture, out int result);
        Value = result;
    }

    public int Value { get; }
}