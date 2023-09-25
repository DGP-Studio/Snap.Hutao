// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Windowing;

internal static class WindowExtension
{
    private static readonly ConditionalWeakTable<Window, WindowController> WindowControllers = new();

    public static void InitializeController<TWindow>(this TWindow window, IServiceProvider serviceProvider)
        where TWindow : Window, IWindowOptionsSource
    {
        WindowController windowController = new(window, window.WindowOptions, serviceProvider);
        WindowControllers.Add(window, windowController);
    }
}