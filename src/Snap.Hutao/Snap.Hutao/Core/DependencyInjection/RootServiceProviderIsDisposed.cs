// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection;

[Obsolete("Use DependencyInjection.DisposeDeferral instead")]
[Injection(InjectAs.Singleton, typeof(IRootServiceProviderIsDisposed))]
internal sealed partial class RootServiceProviderIsDisposed : IRootServiceProviderIsDisposed, IDisposable
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