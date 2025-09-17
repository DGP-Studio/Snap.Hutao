// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using Snap.Hutao.Model;
using Snap.Hutao.Service;
using System.Collections.Immutable;

namespace Snap.Hutao.Core;

internal static partial class Property
{
    public static T Get<T>(this IProperty<T> property)
    {
        return property.Value;
    }

    public static T Set<T>(this IProperty<T> property, T value)
    {
        return property.Value = value;
    }

    public static IObservableProperty<T> CreateObservable<T>(T value)
    {
        return new ObservableProperty<T>(value);
    }

    public static IObservableProperty<T> Observe<TSource, T>(IObservableProperty<TSource> source, Func<TSource, T> converter)
    {
        return new PropertyListener<TSource, T>(source, converter);
    }

    public static IObservableProperty<TSource> Link<TSource, TTarget>(this IObservableProperty<TSource> source, IProperty<TTarget> target, [RequireStaticDelegate] Action<TSource, IProperty<TTarget>> callback)
    {
        return new PropertyLinker<TSource, TTarget>(source, target, callback);
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

    public static IObservableProperty<T> WithValueChangedCallback<T>(this IObservableProperty<T> source, [RequireStaticDelegate] Action<T> callback)
    {
        return new PropertyValueChangedCallbackWrapper<T>(source, callback);
    }

    public static IObservableProperty<T> WithValueChangedCallback<T, TState>(this IObservableProperty<T> source, [RequireStaticDelegate] Action<T, TState> callback, TState state)
    {
        return new PropertyValueChangedCallbackWrapper<T, TState>(source, callback, state);
    }

    public static IObservableProperty<NameValue<T>?> AsNameValue<T>(this IProperty<T> source, ImmutableArray<NameValue<T>> array)
        where T : notnull
    {
        return new PropertyNameValueWrapper<T>(source, array);
    }

    public static IObservableProperty<T?> AsSelection<T, TSource>(this IProperty<TSource> source, ImmutableArray<T> array, Func<T, TSource> valueSelector, IEqualityComparer<TSource> equalityComparer)
        where T : class
    {
        return new PropertySelectionWrapper<T, TSource>(source, array, valueSelector, equalityComparer);
    }

    private sealed partial class PropertyLinker<TSource, TTarget> : ObservableObject, IObservableProperty<TSource>
    {
        private readonly IObservableProperty<TSource> source;
        private readonly IProperty<TTarget> target;
        private readonly Action<TSource, IProperty<TTarget>> callback;

        public PropertyLinker(IObservableProperty<TSource> source, IProperty<TTarget> target, Action<TSource, IProperty<TTarget>> callback)
        {
            this.source = source;
            this.target = target;
            this.callback = callback;

            this.source.WeakPropertyChanged(this, OnWeakSourceValueChanged);
        }

        public TSource Value
        {
            get => source.Value;
            set => SetProperty(source.Value, value, source, static (prop, v) => prop.Value = v);
        }

        private static void OnWeakSourceValueChanged(PropertyLinker<TSource, TTarget> self, object? sender, PropertyChangedEventArgs e)
        {
            self.callback(self.source.Value, self.target);
        }
    }

    private sealed partial class PropertyNameValueWrapper<T> : ObservableObject, IObservableProperty<NameValue<T>?>
        where T : notnull
    {
        private readonly IProperty<T> target;
        private readonly ImmutableArray<NameValue<T>> array;

        public PropertyNameValueWrapper(IProperty<T> target, ImmutableArray<NameValue<T>> array)
        {
            this.target = target;
            this.array = array;
        }

        public NameValue<T>? Value
        {
            get => field ??= Selection.Initialize(array, target.Value);
            set
            {
                if (SetProperty(ref field, value) && value is not null)
                {
                    target.Value = value.Value;
                }
            }
        }
    }

    private sealed partial class PropertySelectionWrapper<T, TSource> : ObservableObject, IObservableProperty<T?>
        where T : class
    {
        private readonly IProperty<TSource> source;
        private readonly ImmutableArray<T> array;
        private readonly Func<T, TSource> valueSelector;
        private readonly IEqualityComparer<TSource> equalityComparer;

        public PropertySelectionWrapper(IProperty<TSource> source, ImmutableArray<T> array, Func<T, TSource> valueSelector, IEqualityComparer<TSource> equalityComparer)
        {
            this.source = source;
            this.array = array;
            this.valueSelector = valueSelector;
            this.equalityComparer = equalityComparer;
        }

        public T? Value
        {
            get => field ??= Selection.Initialize(array, source.Value, valueSelector, equalityComparer);
            set
            {
                if (SetProperty(ref field, value) && value is not null)
                {
                    source.Value = valueSelector(value);
                }
            }
        }
    }

    private sealed partial class PropertyValueChangedCallbackWrapper<T> : ObservableObject, IObservableProperty<T>
    {
        private readonly IObservableProperty<T> source;
        private readonly Action<T> callback;

        public PropertyValueChangedCallbackWrapper(IObservableProperty<T> source, Action<T> callback)
        {
            this.source = source;
            this.callback = callback;

            this.source.WeakPropertyChanged(this, OnWeakSourceValueChanged);
        }

        public T Value
        {
            get => source.Value;
            set => SetProperty(source.Value, value, source, static (prop, v) => prop.Value = v);
        }

        private static void OnWeakSourceValueChanged(PropertyValueChangedCallbackWrapper<T> self, object? sender, PropertyChangedEventArgs e)
        {
            self.callback(self.source.Value);
        }
    }

    private sealed partial class PropertyValueChangedCallbackWrapper<T, TState> : ObservableObject, IObservableProperty<T>
    {
        private readonly IObservableProperty<T> source;
        private readonly Action<T, TState> callback;
        private readonly TState state;

        public PropertyValueChangedCallbackWrapper(IObservableProperty<T> source, Action<T, TState> callback, TState state)
        {
            this.source = source;
            this.callback = callback;
            this.state = state;

            this.source.WeakPropertyChanged(this, OnWeakSourceValueChanged);
        }

        public T Value
        {
            get => source.Value;
            set => SetProperty(source.Value, value, source, static (prop, v) => prop.Value = v);
        }

        private static void OnWeakSourceValueChanged(PropertyValueChangedCallbackWrapper<T, TState> self, object? sender, PropertyChangedEventArgs e)
        {
            self.callback(self.source.Value, self.state);
        }
    }

    private sealed partial class PropertyListener<TSource, T> : ObservableObject, IObservableProperty<T>
    {
        private readonly IObservableProperty<TSource> source;
        private readonly Func<TSource, T> converter;

        public PropertyListener(IObservableProperty<TSource> source, Func<TSource, T> converter)
        {
            this.source = source;
            this.converter = converter;

            this.source.WeakPropertyChanged(this, OnWeakSourceValueChanged);
        }

        [field: MaybeNull]
        public T Value
        {
            get => field ??= converter(source.Value);
            set => SetProperty(ref field, value);
        }

        private static void OnWeakSourceValueChanged(PropertyListener<TSource, T> self, object? sender, PropertyChangedEventArgs e)
        {
            self.Value = self.converter(self.source.Value);
        }
    }
}