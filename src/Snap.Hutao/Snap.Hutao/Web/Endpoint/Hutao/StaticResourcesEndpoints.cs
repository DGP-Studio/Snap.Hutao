// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hutao;

internal static class StaticResourcesEndpoints
{
    public static string Root { get; } = "https://api.snapgenshin.com";

    public static Uri UIIconNone { get; } = StaticRaw("Bg", "UI_Icon_None.png").ToUri();

    public static Uri UIItemIconNone { get; } = StaticRaw("Bg", "UI_ItemIcon_None.png").ToUri();

    public static Uri UIAvatarIconSideNone { get; } = StaticRaw("AvatarIcon", "UI_AvatarIcon_Side_None.png").ToUri();

    public static string StaticRaw(string category, string fileName)
    {
        return $"{Root}/static/raw/{category}/{fileName}";
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