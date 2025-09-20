// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;

namespace Snap.Hutao.Core.Property;

internal static class ReadOnlyProperty
{
    public static T Get<T>(this IReadOnlyProperty<T> property)
    {
        return property.Value;
    }

    public static IReadOnlyObservableProperty<T> Debug<T>(this IReadOnlyObservableProperty<T> source, string name)
    {
        return new ReadOnlyObservablePropertyDebug<T>(source, name);
    }

    public static IReadOnlyObservableProperty<T> WithValueChangedCallback<T>(this IReadOnlyObservableProperty<T> source, [RequireStaticDelegate] Action<T> callback)
    {
        return new ReadOnlyPropertyValueChangedCallbackWrapper<T>(source, callback);
    }

    public static IReadOnlyObservableProperty<T> WithValueChangedCallback<T, TState>(this IReadOnlyObservableProperty<T> source, [RequireStaticDelegate] Action<T, TState> callback, TState state)
    {
        return new ReadOnlyPropertyValueChangedCallbackWrapper<T, TState>(source, callback, state);
    }
}