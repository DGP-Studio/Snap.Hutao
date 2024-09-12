﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.ViewModel.AvatarProperty;

internal class ReliquarySubProperty
{
    public ReliquarySubProperty(FightProperty type, string value)
    {
        Name = type.GetLocalizedDescription();
        Value = value;
    }

    public string Name { get; }

    public string Value { get; }
}