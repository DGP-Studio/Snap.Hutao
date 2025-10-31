// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Property;
using Snap.Hutao.Model;
using Snap.Hutao.Service.Abstraction.Property;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Abstraction;

internal abstract partial class DbStoreOptions : ObservableObject
{
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial DbStoreOptions(IServiceProvider serviceProvider);

    protected IObservableProperty<string> CreateProperty(string key, string defaultValue)
    {
        return new StringDbProperty(serviceProvider, key, defaultValue);
    }

    protected IObservableProperty<string?> CreateProperty(string key)
    {
        return new NullableStringDbProperty(serviceProvider, key);
    }

    protected IObservableProperty<bool> CreateProperty(string key, bool defaultValue)
    {
        return new BooleanDbProperty(serviceProvider, key, defaultValue);
    }

    protected IObservableProperty<int> CreateProperty(string key, int defaultValue)
    {
        return new Int32DbProperty(serviceProvider, key, defaultValue);
    }

    protected IObservableProperty<int> CreateProperty(string key, Func<int> defaultValueFactory)
    {
        return new Int32DbProperty(serviceProvider, key, defaultValueFactory);
    }

    protected IObservableProperty<float> CreateProperty(string key, float defaultValue)
    {
        return new SingleDbProperty(serviceProvider, key, defaultValue);
    }

    protected IObservableProperty<TEnum> CreateProperty<TEnum>(string key, TEnum defaultValue)
        where TEnum : struct, Enum
    {
        return new EnumDbProperty<TEnum>(serviceProvider, key, defaultValue);
    }

    protected IObservableProperty<T> CreatePropertyForStructUsingCustom<T>(string key, T defaultValue, Func<string, T> from, Func<T, string> to)
        where T : struct
    {
        return new StructUsingCustomDbProperty<T>(serviceProvider, key, defaultValue, from, to);
    }

    protected IObservableProperty<T> CreatePropertyForStructUsingJson<T>(string key, T defaultValue)
        where T : struct
    {
        return new StructUsingJsonDbProperty<T>(serviceProvider, key, defaultValue);
    }

    protected IObservableProperty<T> CreatePropertyForClassUsingCustom<T>(string key, T defaultValue, Func<string, T> from, Func<T, string> to)
        where T : class
    {
        return new ClassUsingCustomDbProperty<T>(serviceProvider, key, defaultValue, from, to);
    }

    protected IObservableProperty<NameValue<int>?> CreatePropertyForSelectedOneBasedIndex(string key, ImmutableArray<NameValue<int>> array)
    {
        return new SelectedOneBasedIndexDbProperty(serviceProvider, key, array);
    }
}