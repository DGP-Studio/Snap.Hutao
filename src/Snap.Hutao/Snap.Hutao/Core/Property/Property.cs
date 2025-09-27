// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;
using Snap.Hutao.Model;
using System.Collections.Immutable;

namespace Snap.Hutao.Core.Property;

internal static class Property
{
    public static T Get<T>(this IProperty<T> property)
    {
        return property.Value;
    }

    public static T Set<T>(this IProperty<T> property, T value)
    {
        return property.Value = value;
    }

    public static IObservableProperty<T> Debug<T>(this IObservableProperty<T> property, string name)
    {
        return new ObservablePropertyDebug<T>(property, name);
    }

    public static IProperty<T> Create<T>(T value)
    {
        return new Property<T>(value);
    }

    public static IObservableProperty<T> CreateObservable<T>(T value)
    {
        return new ObservableProperty<T>(value);
    }

    public static IReadOnlyObservableProperty<T> Observe<TSource, T>(IObservableProperty<TSource> source, Func<TSource, T> converter)
    {
        return new PropertyObserver<TSource, T>(source, converter);
    }

    public static IObservableProperty<TSource> Link<TSource, TTarget>(this IObservableProperty<TSource> source, IProperty<TTarget> target, [RequireStaticDelegate] Action<TSource, IProperty<TTarget>> callback)
    {
        return new PropertyValueChangedCallbackWrapper<TSource, IProperty<TTarget>>(source, callback, target);
    }

    public static IObservableProperty<bool> AlsoSetFalseWhenFalse(this IObservableProperty<bool> source, IProperty<bool> target)
    {
        return Link(source, target, static (value, target) =>
        {
            if (!value)
            {
                target.Value = false;
            }
        });
    }

    public static IObservableProperty<T> SetWithCondition<T, TState>(this IObservableProperty<T> source, Func<T, TState, bool> condition, TState state)
    {
        return new ObservablePropertyWithConditionalSetMethod<T, TState>(source, condition, state);
    }

    public static IObservableProperty<T> WithValueChangedCallback<T>(this IObservableProperty<T> source, [RequireStaticDelegate] Action<T> callback)
    {
        return new PropertyValueChangedCallbackWrapper<T>(source, callback);
    }

    public static IObservableProperty<T> WithValueChangedCallback<T, TState>(this IObservableProperty<T> source, [RequireStaticDelegate] Action<T, TState> callback, TState state)
    {
        return new PropertyValueChangedCallbackWrapper<T, TState>(source, callback, state);
    }

    public static IObservableProperty<NameValue<T>?> AsNameValue<T>(this IObservableProperty<T> source, ImmutableArray<NameValue<T>> array)
        where T : notnull
    {
        return new PropertyNameValueWrapper<T>(source, array);
    }

    public static IObservableProperty<T?> AsNullableSelection<TSource, T>(this IProperty<TSource> source, ImmutableArray<T> array, Func<T?, TSource> valueSelector, IEqualityComparer<TSource> equalityComparer)
        where T : class
        where TSource : notnull
    {
        return new PropertyNullableSelectionWrapper<T, TSource>(source, array, valueSelector, equalityComparer);
    }

    public static IProperty<bool> Negate(this IProperty<bool> source)
    {
        return new BooleanPropertyNegation(source);
    }
}

[SuppressMessage("", "SA1402")]
internal sealed class Property<T> : IProperty<T>
{
    public Property(T value)
    {
        Value = value;
    }

    public T Value { get; set; }
}