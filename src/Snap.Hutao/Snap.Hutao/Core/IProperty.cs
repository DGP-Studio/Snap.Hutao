// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core;

internal interface IProperty<T>
{
    T Value { get; set; }
}