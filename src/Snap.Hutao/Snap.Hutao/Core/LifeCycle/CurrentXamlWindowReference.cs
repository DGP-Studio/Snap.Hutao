// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.Core.LifeCycle;

[Injection(InjectAs.Singleton, typeof(ICurrentXamlWindowReference))]
internal sealed class CurrentXamlWindowReference : ICurrentXamlWindowReference
{
    private readonly WeakReference<Window> reference = new(default!);

    [SuppressMessage("", "SH007")]
    public Window? Window
    {
        get
        {
            reference.TryGetTarget(out Window? window);
            return window!;
        }
        set => reference.SetTarget(value!);
    }
}