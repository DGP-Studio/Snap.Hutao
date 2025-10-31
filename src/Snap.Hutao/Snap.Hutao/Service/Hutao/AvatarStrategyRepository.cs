// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.Hutao;

[Service(ServiceLifetime.Singleton, typeof(IAvatarStrategyRepository))]
internal sealed partial class AvatarStrategyRepository : IAvatarStrategyRepository
{
    [GeneratedConstructor]
    public partial AvatarStrategyRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public AvatarStrategy? GetStrategyByAvatarId(AvatarId avatarId)
    {
        return this.SingleOrDefault(s => s.AvatarId == (uint)avatarId);
    }

    public void AddStrategy(AvatarStrategy strategy)
    {
        this.Add(strategy);
    }

    public void RemoveStrategy(AvatarStrategy strategy)
    {
        this.Delete(strategy);
    }
}