// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Database;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Service.Abstraction;

/// <summary>
/// 数据库存储选项的设置
/// </summary>
[ConstructorGenerated]
internal abstract partial class DbStoreOptions : ObservableObject
{
    private readonly IServiceProvider serviceProvider;

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

    [return: NotNull]
    protected T GetOption<T>(ref T? storage, string key, Func<string, T> deserializer, [DisallowNull] T defaultValue)
    {
        if (storage is not null)
        {
            return storage;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == key)?.Value;
            if (value is null)
            {
                storage = defaultValue;
            }
            else
            {
                T targetValue = deserializer(value);
                ArgumentNullException.ThrowIfNull(targetValue);
                storage = targetValue;
            }
        }

        return storage;
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

    protected void SetOption(ref string? storage, string key, string? value, [CallerMemberName] string? propertyName = null)
    {
        if (!SetProperty(ref storage, value, propertyName))
        {
            return;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == key);
            appDbContext.Settings.AddAndSave(new(key, value));
        }
    }

    protected bool SetOption(ref bool? storage, string key, bool value, [CallerMemberName] string? propertyName = null)
    {
        bool set = SetProperty(ref storage, value, propertyName);
        if (!set)
        {
            return set;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == key);
            appDbContext.Settings.AddAndSave(new(key, value.ToString()));
        }

        return set;
    }

    protected void SetOption(ref int? storage, string key, int value, [CallerMemberName] string? propertyName = null)
    {
        if (!SetProperty(ref storage, value, propertyName))
        {
            return;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == key);
            appDbContext.Settings.AddAndSave(new(key, $"{value}"));
        }
    }

    protected void SetOption<T>(ref T? storage, string key, T value, Func<T, string> serializer, [CallerMemberName] string? propertyName = null)
    {
        if (!SetProperty(ref storage, value, propertyName))
        {
            return;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.Settings.ExecuteDeleteWhere(e => e.Key == key);
            appDbContext.Settings.AddAndSave(new(key, serializer(value)));
        }
    }
}