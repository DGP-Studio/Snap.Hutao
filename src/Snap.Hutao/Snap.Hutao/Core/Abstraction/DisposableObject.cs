// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Abstraction;

[HighQuality]
[SuppressMessage("", "SA1600")]
internal abstract class DisposableObject : IDisposable
{
    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
        if (!IsDisposed)
        {
            GC.SuppressFinalize(this);
            Dispose(isDisposing: true);
        }
    }

    protected virtual void Dispose(bool isDisposing)
    {
        IsDisposed = true;
    }

    protected void VerifyNotDisposed()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
