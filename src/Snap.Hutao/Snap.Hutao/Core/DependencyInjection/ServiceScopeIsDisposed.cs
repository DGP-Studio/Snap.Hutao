// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection;

[Obsolete("Use DependencyInjection.DisposeDeferral instead")]
[Injection(InjectAs.Scoped, typeof(IServiceScopeIsDisposed))]
internal sealed partial class ServiceScopeIsDisposed : IServiceScopeIsDisposed, IDisposable
{
    private volatile bool isDisposed;

    public bool IsDisposed { get => isDisposed; }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        isDisposed = true;
    }
}