// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Control.Extension;

internal static class DependencyObjectExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IServiceProvider ServiceProvider(this DependencyObject obj)
    {
        return Ioc.Default;
    }
}