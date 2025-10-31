// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;
using System.Collections.Immutable;
using CalculableAvatar = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Avatar;
using CalculableWeapon = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Weapon;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataWeapon = Snap.Hutao.Model.Metadata.Weapon.Weapon;

namespace Snap.Hutao.Service.Inventory;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class PromotionDeltaFactory
{
    private readonly ILogger<PromotionDeltaFactory> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly IMemoryCache memoryCache;

    [GeneratedConstructor]
    public partial PromotionDeltaFactory(IServiceProvider serviceProvider);

    public async ValueTask<ImmutableArray<AvatarPromotionDelta>> GetAsync(ICultivationMetadataContext context, UserAndUid userAndUid)
    {
        ImmutableArray<AvatarPromotionDelta> result = await memoryCache.GetOrCreateAsync($"{nameof(PromotionDeltaFactory)}.Cache", async entry =>
        {
            ImmutableArray<CalculableAvatar> calculableAvatars;
            ImmutableArray<CalculableWeapon> calculableWeapons;
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                CalculateClient calculateClient = scope.ServiceProvider.GetRequiredService<CalculateClient>();
                calculableAvatars = await calculateClient.GetAllAvatarsAsync(userAndUid).ConfigureAwait(false);
                calculableWeapons = await calculateClient.GetAllWeaponsAsync(userAndUid).ConfigureAwait(false);
            }

            ImmutableArray<ICultivationItemsAccess> cultivationItemsEntryList = Create(context, calculableAvatars, calculableWeapons).Sort(CultivationItemsAccessComparer.Shared);

            using (ValueStopwatch.MeasureExecution(logger))
            {
                return ToPromotionDeltaArray(cultivationItemsEntryList);
            }
        }).ConfigureAwait(false);

        return result;
    }

    private static ImmutableArray<ICultivationItemsAccess> Create(ICultivationMetadataContext context, ImmutableArray<CalculableAvatar> avatars, ImmutableArray<CalculableWeapon> weapons)
    {
        ImmutableArray<ICultivationItemsAccess>.Builder cultivationItems = ImmutableArray.CreateBuilder<ICultivationItemsAccess>(avatars.Length + weapons.Length);
        foreach (ref readonly CalculableAvatar item in avatars.AsSpan())
        {
            if (context.IdAvatarMap.TryGetValue(item.Id, out MetadataAvatar? avatar))
            {
                cultivationItems.Add(avatar);
            }
        }

        foreach (ref readonly CalculableWeapon item in weapons.AsSpan())
        {
            if (context.IdWeaponMap.TryGetValue(item.Id, out MetadataWeapon? weapon))
            {
                cultivationItems.Add(weapon);
            }
        }

        return cultivationItems.ToImmutable();
    }

    private static ImmutableArray<AvatarPromotionDelta> ToPromotionDeltaArray(ImmutableArray<ICultivationItemsAccess> cultivationItems)
    {
        List<AvatarPromotionDelta> deltas = [];
        int currentWeaponEmptyAvatarIndex = 0;

        foreach (ref readonly ICultivationItemsAccess item in cultivationItems.AsSpan())
        {
            switch (item)
            {
                case MetadataAvatar avatar:
                    deltas.Add(AvatarPromotionDelta.CreateForAvatarMaxConsumption(avatar));

                    break;

                case MetadataWeapon weapon:
                    AvatarPromotionDelta delta;
                    if (currentWeaponEmptyAvatarIndex < deltas.Count)
                    {
                        delta = deltas[currentWeaponEmptyAvatarIndex++];
                    }
                    else
                    {
                        delta = new();
                        deltas.Add(delta);
                    }

                    delta.Weapon = new()
                    {
                        Id = weapon.Id,
                        LevelCurrent = 1,
                        LevelTarget = 90,
                    };

                    break;
            }
        }

        return [.. deltas];
    }

    private sealed class CultivationItemsAccessComparer : IComparer<ICultivationItemsAccess>
    {
        private static readonly LazySlim<CultivationItemsAccessComparer> LazyShared = new(() => new());

        private CultivationItemsAccessComparer()
        {
        }

        public static CultivationItemsAccessComparer Shared { get => LazyShared.Value; }

        public int Compare(ICultivationItemsAccess? x, ICultivationItemsAccess? y)
        {
            return (x, y) switch
            {
                (MetadataAvatar, MetadataWeapon) => -1,
                (MetadataWeapon, MetadataAvatar) => 1,
                _ => 0,
            };
        }
    }
}