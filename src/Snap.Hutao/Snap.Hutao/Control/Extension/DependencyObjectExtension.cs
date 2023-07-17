// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.Control.Extension;

internal static class DependencyObjectExtension
{
    public static IServiceProvider ServiceProvider(this DependencyObject obj)
    {
        return Ioc.Default;
    }
}