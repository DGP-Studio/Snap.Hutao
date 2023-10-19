// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.Core.LifeCycle;

internal static class CurrentWindowReferenceExtension
{
    public static XamlRoot GetXamlRoot(this ICurrentWindowReference reference)
    {
        return reference.Window.Content.XamlRoot;
    }
}
