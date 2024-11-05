// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.GachaLog;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IGachaLogWishCountdownService))]
internal sealed partial class GachaLogWishCountdownService : IGachaLogWishCountdownService
{
    private readonly IMetadataService metadataService;

    private GachaLogWishCountdownServiceMetadataContext context;

    public async ValueTask<bool> InitializeAsync()
    {
        if (context is { IsInitialized: true })
        {
            return true;
        }

        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            context = await metadataService.GetContextAsync<GachaLogWishCountdownServiceMetadataContext>()
                .ConfigureAwait(false);
            return true;
        }

        return false;
    }

    public async ValueTask<ValueResult<bool, WishCountdowns>> GetWishCountdownsAsync()
    {
        if (!await InitializeAsync().ConfigureAwait(false))
        {
            return new(false, default!);
        }

        return new(true, GetWishCountdownsCore());
    }

    private WishCountdowns GetWishCountdownsCore()
    {
        HashSet<uint> ids = [];

        List<Countdown> orangeAvatarCountdowns = [];
        List<Countdown> purpleAvatarCountdowns = [];
        List<Countdown> orangeWeaponCountdowns = [];
        List<Countdown> purpleWeaponCountdowns = [];
        foreach (GachaEvent gachaEvent in context.GachaEvents.Reverse())
        {
            if (gachaEvent.Type is GachaType.ActivityAvatar or GachaType.SpecialActivityAvatar)
            {
                foreach (uint avatarId in gachaEvent.UpOrangeList)
                {
                    if (!AvatarIds.IsStandardWish(avatarId) && ids.Add(avatarId))
                    {
                        orangeAvatarCountdowns.Insert(0, new(context.GetAvatar(avatarId).ToItem<Item>(), gachaEvent));
                    }
                }

                foreach (uint avatarId in gachaEvent.UpPurpleList)
                {
                    if (ids.Add(avatarId))
                    {
                        purpleAvatarCountdowns.Insert(0, new(context.GetAvatar(avatarId).ToItem<Item>(), gachaEvent));
                    }
                }

                continue;
            }

            if (gachaEvent.Type is GachaType.ActivityWeapon)
            {
                foreach (uint weaponId in gachaEvent.UpOrangeList)
                {
                    if (!WeaponIds.IsStandardWish(weaponId) && ids.Add(weaponId))
                    {
                        orangeWeaponCountdowns.Insert(0, new(context.GetWeapon(weaponId).ToItem<Item>(), gachaEvent));
                    }
                }

                foreach (uint weaponId in gachaEvent.UpPurpleList)
                {
                    if (ids.Add(weaponId))
                    {
                        purpleWeaponCountdowns.Insert(0, new(context.GetWeapon(weaponId).ToItem<Item>(), gachaEvent));
                    }
                }

                continue;
            }

            if (gachaEvent.Type is GachaType.ActivityCity)
            {
                foreach (uint itemId in gachaEvent.UpOrangeList)
                {
                    if (!(AvatarIds.IsStandardWish(itemId) || WeaponIds.IsStandardWish(itemId)) && ids.Add(itemId))
                    {
                        switch (itemId.StringLength())
                        {
                            case 8U:
                                orangeAvatarCountdowns.Insert(0, new(context.GetAvatar(itemId).ToItem<Item>(), gachaEvent));
                                break;
                            case 5U:
                                orangeWeaponCountdowns.Insert(0, new(context.GetWeapon(itemId).ToItem<Item>(), gachaEvent));
                                break;
                            default:
                                throw HutaoException.NotSupported();
                        }
                    }
                }

                foreach (uint itemId in gachaEvent.UpPurpleList)
                {
                    if (ids.Add(itemId))
                    {
                        switch (itemId.StringLength())
                        {
                            case 8U:
                                purpleAvatarCountdowns.Insert(0, new(context.GetAvatar(itemId).ToItem<Item>(), gachaEvent));
                                break;
                            case 5U:
                                purpleWeaponCountdowns.Insert(0, new(context.GetWeapon(itemId).ToItem<Item>(), gachaEvent));
                                break;
                            default:
                                throw HutaoException.NotSupported();
                        }
                    }
                }
            }
        }

        return new()
        {
            OrangeAvatars = orangeAvatarCountdowns,
            PurpleAvatars = purpleAvatarCountdowns,
            OrangeWeapons = orangeWeaponCountdowns,
            PurpleWeapons = purpleWeaponCountdowns,
        };
    }
}