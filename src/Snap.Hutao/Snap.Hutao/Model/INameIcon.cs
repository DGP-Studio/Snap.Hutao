// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model;

internal interface INameIcon<out TIcon>
{
    string Name { get; }

    TIcon Icon { get; }
}