// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

internal readonly ref struct VolatileWrite
{
    private readonly ref bool reference;
    private readonly bool initialState;

    public VolatileWrite(ref bool reference, bool initialState)
    {
        this.reference = ref reference;
        this.initialState = initialState;
        Volatile.Write(ref this.reference, initialState);
    }

    public void Dispose()
    {
        Volatile.Write(ref reference, !initialState);
    }
}