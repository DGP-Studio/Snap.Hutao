// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Web.Hutao.GachaLog;

internal sealed class EndIds
{
    [JsonPropertyName("100")]
    public long NoviceWish { get; set; }

    [JsonPropertyName("200")]
    public long StandardWish { get; set; }

    [JsonPropertyName("301")]
    public long AvatarEventWish { get; set; }

    [JsonPropertyName("302")]
    public long WeaponEventWish { get; set; }

    [JsonPropertyName("500")]
    public long ChronicledWish { get; set; }

    public long this[GachaType type]
    {
        get
        {
            return type switch
            {
                GachaType.NewBie => NoviceWish,
                GachaType.Standard => StandardWish,
                GachaType.ActivityAvatar => AvatarEventWish,
                GachaType.ActivityWeapon => WeaponEventWish,
                GachaType.ActivityCity => ChronicledWish,
                _ => 0,
            };
        }

        set
        {
            switch (type)
            {
                case GachaType.NewBie:
                    NoviceWish = value;
                    break;
                case GachaType.Standard:
                    StandardWish = value;
                    break;
                case GachaType.ActivityAvatar:
                    AvatarEventWish = value;
                    break;
                case GachaType.ActivityWeapon:
                    WeaponEventWish = value;
                    break;
                case GachaType.ActivityCity:
                    ChronicledWish = value;
                    break;
            }
        }
    }

    public IEnumerator<KeyValuePair<GachaType, long>> GetEnumerator()
    {
        yield return new(GachaType.NewBie, NoviceWish);
        yield return new(GachaType.Standard, StandardWish);
        yield return new(GachaType.ActivityAvatar, AvatarEventWish);
        yield return new(GachaType.ActivityWeapon, WeaponEventWish);
        yield return new(GachaType.ActivityCity, ChronicledWish);
    }
}