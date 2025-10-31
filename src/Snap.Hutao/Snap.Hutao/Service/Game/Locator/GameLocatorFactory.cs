// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Locator;

[Service(ServiceLifetime.Singleton, typeof(IGameLocatorFactory))]
internal sealed partial class GameLocatorFactory : IGameLocatorFactory
{
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial GameLocatorFactory(IServiceProvider serviceProvider);

    public IGameLocator Create(GameLocationSourceKind source)
    {
        return serviceProvider.GetRequiredKeyedService<IGameLocator>(source);
    }
}