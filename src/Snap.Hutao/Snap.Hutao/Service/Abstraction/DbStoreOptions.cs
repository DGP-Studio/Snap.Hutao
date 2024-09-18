// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Service.Abstraction;

[ConstructorGenerated]
internal abstract partial class DbStoreOptions : ObservableObject
{
    private readonly IServiceProvider serviceProvider;

    protected static T? EnumParse<T>(string input)
        where T : struct, Enum
    {
        return Enum.Parse<T>(input);
    }

    protected static string EnumToStringOrEmpty<T>(T? input)
        where T : struct, Enum
    {
        return input.ToStringOrEmpty();
    }

    protected void InitializeOptions(Expression<Func<SettingEntry, bool>> entrySelector, Action<string, string?> entryAction)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            foreach (SettingEntry entry in appDbContext.Settings.Where(entrySelector))
            {
                entryAction(entry.Key, entry.Value);
            }
        }
    }

    protected string GetOption(ref string? storage, string key, string defaultValue = "")
    {
        return GetOption(ref storage, key, () => defaultValue);
    }

    protected string GetOption(ref string? storage, string key, Func<string> defaultValueFactory)
    {
        if (storage is not null)
        {
            return storage;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            storage = appDbContext.Settings.SingleOrDefault(e => e.Key == key)?.Value ?? defaultValueFactory();
        }

        return storage;
    }

    protected bool GetOption(ref bool? storage, string key, bool defaultValue = false)
    {
        return GetOption(ref storage, key, () => defaultValue);
    }

    protected bool GetOption(ref bool? storage, string key, Func<bool> defaultValueFactory)
    {
        if (storage is not null)
        {
            return storage.Value;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == key)?.Value;
            storage = value is null ? defaultValueFactory() : bool.Parse(value);
        }

        return storage.Value;
    }

    protected int GetOption(ref int? storage, string key, int defaultValue = 0)
    {
        return GetOption(ref storage, key, () => defaultValue);
    }

    protected int GetOption(ref int? storage, string key, Func<int> defaultValueFactory)
    {
        if (storage is not null)
        {
            return storage.Value;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == key)?.Value;
            storage = value is null ? defaultValueFactory() : int.Parse(value, CultureInfo.InvariantCulture);
        }

        return storage.Value;
    }

    protected float GetOption(ref float? storage, string key, float defaultValue = 0f)
    {
        return GetOption(ref storage, key, () => defaultValue);
    }

    protected float GetOption(ref float? storage, string key, Func<float> defaultValueFactory)
    {
        if (storage is not null)
        {
            return storage.Value;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == key)?.Value;
            storage = value is null ? defaultValueFactory() : float.Parse(value, CultureInfo.InvariantCulture);
        }

        return storage.Value;
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    protected T GetOption<T>(ref T? storage, string key, Func<string, T> deserializer, T defaultValue)
    {
        return GetOption(ref storage, key, deserializer, () => defaultValue);
    }

    protected T GetOption<T>(ref T? storage, string key, Func<string, T> deserializer, Func<T> defaultValueFactory)
    {
        if (storage is not null)
        {
            return storage;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == key)?.Value;
            storage = value is null ? defaultValueFactory() : deserializer(value);
        }

        return storage;
    }

    protected bool SetOption(ref string? storage, string key, string? value, [CallerMemberName] string? propertyName = null)
    {
        return SetOption(ref storage, key, value, v => v, propertyName);
    }

    protected bool SetOption(ref bool? storage, string key, bool value, [CallerMemberName] string? propertyName = null)
    {
        return SetOption(ref storage, key, value, v => $"{v}", propertyName);
    }

    protected bool SetOption(ref int? storage, string key, int value, [CallerMemberName] string? propertyName = null)
    {
        return SetOption(ref storage, key, value, v => $"{v}", propertyName);
    }

    protected bool SetOption(ref float? storage, string key, float value, [CallerMemberName] string? propertyName = null)
    {
        return SetOption(ref storage, key, value, v => $"{v}", propertyName);
    }

    protected bool SetOption<T>(ref T? storage, string key, T value, Func<T, string?> serializer, [CallerMemberName] string? propertyName = null)
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
}