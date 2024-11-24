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

    private GachaLogWishCountdownServiceMetadataContext? context;

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
        ArgumentNullException.ThrowIfNull(context);

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
                    if (!AvatarIds.IsStandardWish(avatarId))
                    {
                        Countdown countdown;
                        if (ids.Add(avatarId))
                        {
                            countdown = new(context.GetAvatar(avatarId).ToItem<Item>());
                            orangeAvatarCountdowns.Insert(0, countdown);
                        }
                        else
                        {
                            countdown = orangeAvatarCountdowns.Single(c => c.Item.Id == avatarId);
                        }

                        countdown.Histories.Add(new(gachaEvent));
                    }
                }

                foreach (uint avatarId in gachaEvent.UpPurpleList)
                {
                    Countdown countdown;
                    if (ids.Add(avatarId))
                    {
                        countdown = new(context.GetAvatar(avatarId).ToItem<Item>());
                        purpleAvatarCountdowns.Insert(0, countdown);
                    }
                    else
                    {
                        countdown = purpleAvatarCountdowns.Single(c => c.Item.Id == avatarId);
                    }

                    countdown.Histories.Add(new(gachaEvent));
                }

                continue;
            }

            if (gachaEvent.Type is GachaType.ActivityWeapon)
            {
                foreach (uint weaponId in gachaEvent.UpOrangeList)
                {
                    if (!WeaponIds.IsStandardWish(weaponId))
                    {
                        Countdown countdown;
                        if (ids.Add(weaponId))
                        {
                            countdown = new(context.GetWeapon(weaponId).ToItem<Item>());
                            orangeWeaponCountdowns.Insert(0, countdown);
                        }
                        else
                        {
                            countdown = orangeWeaponCountdowns.Single(c => c.Item.Id == weaponId);
                        }

                        countdown.Histories.Add(new(gachaEvent));
                    }
                }

                foreach (uint weaponId in gachaEvent.UpPurpleList)
                {
                    Countdown countdown;
                    if (ids.Add(weaponId))
                    {
                        countdown = new(context.GetWeapon(weaponId).ToItem<Item>());
                        purpleWeaponCountdowns.Insert(0, countdown);
                    }
                    else
                    {
                        countdown = purpleWeaponCountdowns.Single(c => c.Item.Id == weaponId);
                    }

                    countdown.Histories.Add(new(gachaEvent));
                }

                continue;
            }

            if (gachaEvent.Type is GachaType.ActivityCity)
            {
                foreach (uint itemId in gachaEvent.UpOrangeList)
                {
                    if (!(AvatarIds.IsStandardWish(itemId) || WeaponIds.IsStandardWish(itemId)))
                    {
                        Countdown countdown;
                        switch (itemId.StringLength())
                        {
                            case 8U:
                                if (ids.Add(itemId))
                                {
                                    countdown = new(context.GetAvatar(itemId).ToItem<Item>());
                                    orangeAvatarCountdowns.Insert(0, countdown);
                                }
                                else
                                {
                                    countdown = orangeAvatarCountdowns.Single(c => c.Item.Id == itemId);
                                }

                                break;
                            case 5U:
                                if (ids.Add(itemId))
                                {
                                    countdown = new(context.GetWeapon(itemId).ToItem<Item>());
                                    orangeWeaponCountdowns.Insert(0, countdown);
                                }
                                else
                                {
                                    countdown = orangeWeaponCountdowns.Single(c => c.Item.Id == itemId);
                                }

                                break;
                            default:
                                throw HutaoException.NotSupported();
                        }

                        countdown.Histories.Add(new(gachaEvent));
                    }
                }

                foreach (uint itemId in gachaEvent.UpPurpleList)
                {
                    Countdown countdown;
                    switch (itemId.StringLength())
                    {
                        case 8U:
                            if (ids.Add(itemId))
                            {
                                countdown = new(context.GetAvatar(itemId).ToItem<Item>());
                                purpleAvatarCountdowns.Insert(0, countdown);
                            }
                            else
                            {
                                countdown = purpleAvatarCountdowns.Single(c => c.Item.Id == itemId);
                            }

                            break;
                        case 5U:
                            if (ids.Add(itemId))
                            {
                                countdown = new(context.GetWeapon(itemId).ToItem<Item>());
                                purpleWeaponCountdowns.Insert(0, countdown);
                            }
                            else
                            {
                                countdown = purpleWeaponCountdowns.Single(c => c.Item.Id == itemId);
                            }

                            break;
                        default:
                            throw HutaoException.NotSupported();
                    }

                    countdown.Histories.Add(new(gachaEvent));
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