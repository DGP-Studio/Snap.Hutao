// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.Core.LifeCycle;

[Service(ServiceLifetime.Singleton, typeof(ICurrentXamlWindowReference<>))]
internal sealed class CurrentXamlWindowReference<TWindow> : ICurrentXamlWindowReference<TWindow>
    where TWindow : Window
{
    private readonly WeakReference<TWindow> reference = new(default!);

    public TWindow? Window
    {
        get
        {
            reference.TryGetTarget(out TWindow? window);
            return window;
        }

        set => reference.SetTarget(value!);
    }
}