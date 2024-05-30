// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.View.Helper;

internal sealed class DeferContentLoader : IDeferContentLoader
{
    private readonly WeakReference<FrameworkElement> reference = new(default!);

    public DeferContentLoader(FrameworkElement element)
    {
        this.reference.SetTarget(element);
    }

    public void Load(string name)
    {
        if (reference.TryGetTarget(out FrameworkElement? element))
        {
            element.FindName(name);
        }
    }
}