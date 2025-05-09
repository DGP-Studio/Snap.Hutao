// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
namespace Snap.Hutao.Core.DependencyInjection.Implementation;

internal sealed class ServiceProviderDisposedException : ObjectDisposedException
{
    public ServiceProviderDisposedException(bool isRootScope)
        : base(isRootScope ? "Root ServiceProvider is disposed" : "ServiceScope is disposed")
    {
        IsRootScope = isRootScope;
    }

    public bool IsRootScope { get; }
}