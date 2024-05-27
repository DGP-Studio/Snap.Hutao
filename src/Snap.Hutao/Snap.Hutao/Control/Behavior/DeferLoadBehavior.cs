// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Control.Behavior;

[DependencyProperty("ElementNames", typeof(DeferLoadCollection))]
internal sealed partial class DeferLoadBehavior : BehaviorBase<FrameworkElement>
{
    protected override bool Initialize()
    {
        if (ElementNames.IsNullOrEmpty())
        {
            return true;
        }

        ThreadPool.UnsafeQueueUserWorkItem(LoadElements, this);
        return true;
    }

    private static void LoadElements(object? state)
    {
        if (state is not DeferLoadBehavior behavior)
        {
            return;
        }

        List<string>? elementNames = null;
        behavior.AssociatedObject.DispatcherQueue.Invoke(() => elementNames = [.. behavior.ElementNames]);

        foreach (string name in CollectionsMarshal.AsSpan(elementNames))
        {
            Thread.Sleep(1000);

            behavior.AssociatedObject.DispatcherQueue.TryEnqueue(() =>
            {
                behavior.AssociatedObject.FindName(name);
            });
        }
    }
}