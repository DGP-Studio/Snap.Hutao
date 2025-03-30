// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Content;
using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Content;

internal static class XamlContextExtension
{
    public static XamlContext XamlContext(this ContentIsland contentIsland)
    {
        return (XamlContext)contentIsland.AppData;
    }

    public static XamlContext XamlContext(this XamlRoot xamlRoot)
    {
        return (XamlContext)xamlRoot.ContentIsland.AppData;
    }
}