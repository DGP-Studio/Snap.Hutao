// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;

namespace Snap.Hutao.Core.DependencyInjection.Abstraction;

internal abstract class DriverMediaTypeAwareFactory<TService, TServiceSSD, TServiceHDD> : IDriverMediaTypeAwareFactory<TService>
    where TServiceSSD : notnull, TService
    where TServiceHDD : notnull, TService
{
    private readonly IServiceProvider serviceProvider;

    public DriverMediaTypeAwareFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public virtual TService Create(string path)
    {
        return PhysicalDriver.DangerousGetIsSolidState(path)
            ? serviceProvider.GetRequiredService<TServiceSSD>()
            : serviceProvider.GetRequiredService<TServiceHDD>();
    }
}