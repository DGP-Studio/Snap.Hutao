// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hutao;

internal static class StaticResourcesEndpoints
{
// TODO: Wait for evaluation of ADAPT_NEW_STATIC_ENDPOINT
#if ADAPT_NEW_STATIC_ENDPOINT
    public static string PreviousRoot { get => "https://api.snapgenshin.com"; }
#endif

    public static string Root { get => "https://api.qhy04.com/hutaoimg"; }

    public static Uri UIIconNone { get; } = StaticRaw("Bg", "UI_Icon_None.png").ToUri();

    public static Uri UIItemIconNone { get; } = StaticRaw("Bg", "UI_ItemIcon_None.png").ToUri();

    public static Uri UIAvatarIconSideNone { get; } = StaticRaw("AvatarIcon", "UI_AvatarIcon_Side_None.png").ToUri();

    public static string StaticRaw(string category, string fileName)
    {
        return string.Intern($"{Root}/static/raw/{category}/{fileName}");
    }

    public static string StaticZip(string fileName)
    {
        return $"{Root}/static/zip/{fileName}.zip";
    }

    public static string StaticSize()
    {
        return $"{Root}/static/size";
    }
}