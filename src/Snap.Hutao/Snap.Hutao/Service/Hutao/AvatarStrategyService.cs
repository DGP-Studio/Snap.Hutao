// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Hutao.Strategy;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Hutao;

[Service(ServiceLifetime.Singleton, typeof(IAvatarStrategyService))]
internal sealed partial class AvatarStrategyService : IAvatarStrategyService
{
    private readonly IAvatarStrategyRepository repository;
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial AvatarStrategyService(IServiceProvider serviceProvider);

    public async ValueTask<AvatarStrategy?> GetStrategyByAvatarId(AvatarId avatarId)
    {
        AvatarStrategy? strategy = repository.GetStrategyByAvatarId(avatarId);
        if (strategy is { ChineseStrategyId: 0 } or { OverseaStrategyId: 0 })
        {
            // Re-download the strategy if the strategy is incomplete
            repository.RemoveStrategy(strategy);
            strategy = default;
        }

        if (strategy is null)
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                HutaoStrategyClient strategyClient = scope.ServiceProvider.GetRequiredService<HutaoStrategyClient>();
                Response<ImmutableDictionary<AvatarId, Strategy>> response = await strategyClient.GetStrategyItemAsync(avatarId).ConfigureAwait(false);

                if (ResponseValidator.TryValidate(response, scope.ServiceProvider, out ImmutableDictionary<AvatarId, Strategy>? dictionary))
                {
                    if (!dictionary.TryGetValue(avatarId, out Strategy? data))
                    {
                        return default;
                    }

                    if (data.HoyolabStrategyId is null && data.MysStrategyId is null)
                    {
                        return default;
                    }

                    strategy = new()
                    {
                        AvatarId = avatarId,
                        ChineseStrategyId = data.MysStrategyId ?? 0,
                        OverseaStrategyId = data.HoyolabStrategyId ?? 0,
                    };

                    repository.AddStrategy(strategy);
                }
            }
        }

        return strategy;
    }
}