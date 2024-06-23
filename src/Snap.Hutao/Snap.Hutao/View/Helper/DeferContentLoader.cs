﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Snap.Hutao.UI.Xaml.Control;

namespace Snap.Hutao.View.Helper;

internal sealed class DeferContentLoader : IDeferContentLoader
{
    private readonly WeakReference<FrameworkElement> reference = new(default!);

    public DeferContentLoader(FrameworkElement element)
    {
        reference.SetTarget(element);
    }

    public DependencyObject? Load(string name)
    {
        if (reference.TryGetTarget(out FrameworkElement? element))
        {
            return element.FindName(name) as DependencyObject;
        }

        return default;
    }

    public void Unload(DependencyObject @object)
    {
        if (reference.TryGetTarget(out FrameworkElement? element) && element is ScopedPage scopedPage)
        {
           scopedPage.UnloadObjectOverride(@object);
        }
        else
        {
            XamlMarkupHelper.UnloadObject(@object);
        }
    }
}