// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Locator;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameLocatorFactory))]
internal sealed partial class GameLocatorFactory : IGameLocatorFactory
{
    private readonly IServiceProvider serviceProvider;

    public IGameLocator Create(GameLocationSource source)
    {
        return serviceProvider.GetRequiredKeyedService<IGameLocator>(source);
    }
}