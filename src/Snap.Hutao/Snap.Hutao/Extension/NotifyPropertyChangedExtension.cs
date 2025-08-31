// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Helpers;
using JetBrains.Annotations;

namespace Snap.Hutao.Extension;

internal static class NotifyPropertyChangedExtension
{
    public static WeakEventListener<TInstance, object?, PropertyChangedEventArgs> WeakPropertyChanged<TInstance>(this INotifyPropertyChanged source, TInstance instance, [RequireStaticDelegate] Action<TInstance, object?, PropertyChangedEventArgs> callback)
        where TInstance : class
    {
        WeakEventListener<TInstance, object?, PropertyChangedEventArgs> weakEvent = new(instance)
        {
            OnEventAction = callback,
            OnDetachAction = listener => source.PropertyChanged -= listener.OnEvent,
        };
        source.PropertyChanged += weakEvent.OnEvent;
        return weakEvent;
    }
}