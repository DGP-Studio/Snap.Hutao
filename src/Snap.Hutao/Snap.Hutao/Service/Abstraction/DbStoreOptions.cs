// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Service.Abstraction.Property;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Service.Abstraction;

[ConstructorGenerated]
internal abstract partial class DbStoreOptions : ObservableObject
{
    private static readonly Func<bool> TrueFunc = static () => true;

    private static readonly Func<bool> FalseFunc = static () => false;

    private readonly IServiceProvider serviceProvider;

    protected static string EnumToStringOrEmpty<T>(T? input)
        where T : struct, Enum
    {
        return input.ToStringOrEmpty();
    }

    protected DbProperty<string> CreateProperty(string key, string defaultValue)
    {
        return new StringDbProperty(serviceProvider, key, defaultValue);
    }

    protected DbProperty<string?> CreateProperty(string key)
    {
        return new NullableStringDbProperty(serviceProvider, key);
    }

    protected DbProperty<bool> CreateProperty(string key, bool defaultValue)
    {
        return new BooleanDbProperty(serviceProvider, key, defaultValue);
    }

    protected DbProperty<int> CreateProperty(string key, int defaultValue)
    {
        return new Int32DbProperty(serviceProvider, key, defaultValue);
    }

    protected DbProperty<int> CreateProperty(string key, Func<int> defaultValueFactory)
    {
        return new Int32DbProperty(serviceProvider, key, defaultValueFactory);
    }

    protected DbProperty<float> CreateProperty(string key, float defaultValue)
    {
        return new SingleDbProperty(serviceProvider, key, defaultValue);
    }

    protected DbProperty<TEnum> CreateProperty<TEnum>(string key, TEnum defaultValue)
        where TEnum : struct, Enum
    {
        return new EnumDbProperty<TEnum>(serviceProvider, key, defaultValue);
    }

    protected DbProperty<T> CreatePropertyForStructUsingJson<T>(string key, T defaultValue)
        where T : struct
    {
        return new StructToJsonDbProperty<T>(serviceProvider, key, defaultValue);
    }

    protected DbProperty<NameValue<int>?> CreatePropertyForSelectedOneBasedIndex(string key, ImmutableArray<NameValue<int>> array)
    {
        return new SelectedOneBasedIndexDbProperty(serviceProvider, key, array);
    }

    [Obsolete]
    protected bool GetOption(ref bool? storage, string key, bool defaultValue)
    {
        return GetOption(ref storage, key, defaultValue ? TrueFunc : FalseFunc);
    }

    [Obsolete]
    protected bool GetOption(ref bool? storage, string key, Func<bool> defaultValueFactory)
    {
        return GetOption(ref storage, key, bool.Parse, defaultValueFactory);
    }

    [Obsolete]
    protected int GetOption(ref int? storage, string key, int defaultValue = 0)
    {
        return GetOption(ref storage, key, () => defaultValue);
    }

    [Obsolete]
    protected int GetOption(ref int? storage, string key, Func<int> defaultValueFactory)
    {
        return GetOption(ref storage, key, int.Parse, defaultValueFactory);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    protected T GetOption<T>(ref T? storage, string key, Func<string, T> deserializer, T defaultValue)
        where T : class
    {
        return GetOption(ref storage, key, deserializer, () => defaultValue);
    }

    protected T GetOption<T>(ref T? storage, string key, Func<string, T> deserializer, T defaultValue)
        where T : struct
    {
        return GetOption(ref storage, key, deserializer, () => defaultValue);
    }

    protected T GetOption<T>(ref T? storage, string key, Func<string, T> deserializer, Func<T> defaultValueFactory)
        where T : class
    {
        if (storage is not null)
        {
            return storage;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            string? value = GetValue(appDbContext, key);
            storage = value is null ? defaultValueFactory() : deserializer(value);
        }

        return storage;
    }

    protected T GetOption<T>(ref T? storage, string key, [RequireStaticDelegate] Func<string, T> deserializer, Func<T> defaultValueFactory)
        where T : struct
    {
        if (storage is not null)
        {
            return storage.Value;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            string? value = GetValue(appDbContext, key);
            storage = value is null ? defaultValueFactory() : deserializer(value);
        }

        return storage.Value;
    }

    protected NameValue<T> GetOption<T>(ref NameValue<T>? storage, string key, ImmutableArray<NameValue<T>> array, [RequireStaticDelegate] Func<T, string> serializer, NameValue<T> defaultValue)
    {
        if (storage is not null)
        {
            return storage;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            string? value = GetValue(appDbContext, key);
            storage = value is null ? defaultValue : array.Single(item => serializer(item.Value) == value);
        }

        return storage;
    }

    [Obsolete]
    protected bool SetOption(ref bool? storage, string key, bool value, [CallerMemberName] string? propertyName = null)
    {
        return SetOption(ref storage, key, value, static v => $"{v}", propertyName);
    }

    [Obsolete]
    protected bool SetOption(ref int? storage, string key, int value, [CallerMemberName] string? propertyName = null)
    {
        return SetOption(ref storage, key, value, static v => $"{v}", propertyName);
    }

    protected bool SetOption<T>(ref T? storage, string key, T value, [RequireStaticDelegate] Func<T, string?> serializer, [CallerMemberName] string? propertyName = null)
    {
        if (!SetProperty(ref storage, value, propertyName))
        {
            return false;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.Settings.Where(e => e.Key == key).ExecuteDelete();
            appDbContext.Settings.AddAndSave(new(key, serializer(value)));
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static string? GetValue(AppDbContext appDbContext, string key)
    {
        // This method is separated to avoid implicit capture of the key
        return appDbContext.Settings.SingleOrDefault(e => e.Key == key)?.Value;
    }
}