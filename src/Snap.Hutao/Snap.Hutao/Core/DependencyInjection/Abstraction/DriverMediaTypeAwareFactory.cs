// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;

namespace Snap.Hutao.Core.DependencyInjection.Abstraction;

internal abstract partial class DriverMediaTypeAwareFactory<TService, TServiceSSD, TServiceHDD> : IDriverMediaTypeAwareFactory<TService>
    where TService : notnull
    where TServiceSSD : TService
    where TServiceHDD : TService
{
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial DriverMediaTypeAwareFactory(IServiceProvider serviceProvider);

    public virtual TService Create(string path)
    {
        return (PhysicalDrive.GetIsSolidState(path) ?? false)
            ? serviceProvider.GetRequiredService<TServiceSSD>()
            : serviceProvider.GetRequiredService<TServiceHDD>();
    }
}