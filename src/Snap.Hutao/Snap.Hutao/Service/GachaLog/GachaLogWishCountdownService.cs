// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
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
            context = await metadataService.GetContextAsync<GachaLogWishCountdownServiceMetadataContext>().ConfigureAwait(false);
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

        List<Countdown> orangeAvatarCountdowns = context.IdAvatarMap.Values
            .Where(avatar => avatar.Quality == QualityType.QUALITY_ORANGE)
            .Where(avatar => !AvatarIds.IsStandardWish(avatar.Id))
            .Select(avatar => new Countdown(
                avatar.ToItem<Item>(),
                context.GachaEvents.Last(e => e.UpOrangeList.Any(i => i == avatar.Id))))
            .OrderBy(c => c.LastTime)
            .ToList();

        List<Countdown> purpleAvatarCountdowns = context.IdAvatarMap.Values
            .Where(avatar => avatar.Quality == QualityType.QUALITY_PURPLE)
            .Where(avatar => !AvatarIds.IsStandardWish(avatar.Id))
            .Select(avatar => new Countdown(
                avatar.ToItem<Item>(),
                context.GachaEvents.Last(e => e.UpPurpleList.Any(i => i == avatar.Id))))
            .OrderBy(c => c.LastTime)
            .ToList();

        List<Countdown> orangeWeaponCountdowns = context.IdWeaponMap.Values
            .Where(weapon => weapon.Quality == QualityType.QUALITY_ORANGE)
            .Where(weapon => !WeaponIds.IsStandardWish(weapon.Id))
            .Select(weapon => new Countdown(
                weapon.ToItem<Item>(),
                context.GachaEvents.Last(e => e.UpOrangeList.Any(i => i == weapon.Id))))
            .OrderBy(c => c.LastTime)
            .ToList();

        HashSet<Weapon> purpleWeapons = [];
        foreach (GachaEvent gachaEvent in context.GachaEvents)
        {
            if (gachaEvent.Type is GachaType.ActivityWeapon)
            {
                foreach (uint weaponId in gachaEvent.UpPurpleList)
                {
                    purpleWeapons.Add(context.GetWeapon(weaponId));
                }
            }
        }

        List<Countdown> purpleWeaponCountdowns = purpleWeapons
            .Select(weapon => new Countdown(
                weapon.ToItem<Item>(),
                context.GachaEvents.Last(e => e.UpPurpleList.Any(i => i == weapon.Id))))
            .OrderBy(c => c.LastTime)
            .ToList();

        return new(true, new()
        {
            OrangeAvatars = orangeAvatarCountdowns,
            PurpleAvatars = purpleAvatarCountdowns,
            OrangeWeapons = orangeWeaponCountdowns,
            PurpleWeapons = purpleWeaponCountdowns,
        });
    }

}
