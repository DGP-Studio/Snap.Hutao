// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Collection;

internal interface IReadOnlySpanAccess<T>
{
    ReadOnlySpan<T> ReadOnlySpan { get; }
}