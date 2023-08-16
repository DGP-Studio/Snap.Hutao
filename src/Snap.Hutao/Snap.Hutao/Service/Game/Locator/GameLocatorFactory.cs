// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Locator;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IGameLocatorFactory))]
internal sealed partial class GameLocatorFactory : IGameLocatorFactory
{
    [SuppressMessage("", "SH301")]
    private readonly IServiceProvider serviceProvider;

    public IGameLocator Create(GameLocationSource source)
    {
        return source switch
        {
            GameLocationSource.Registry => serviceProvider.GetRequiredService<RegistryLauncherLocator>(),
            GameLocationSource.UnityLog => serviceProvider.GetRequiredService<UnityLogGameLocator>(),
            GameLocationSource.Manual => serviceProvider.GetRequiredService<ManualGameLocator>(),
            _ => throw Must.NeverHappen(),
        };
    }
}
