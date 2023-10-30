// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Options;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Database;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Service.Abstraction;

/// <summary>
/// 数据库存储选项的设置
/// </summary>
[ConstructorGenerated]
internal abstract partial class DbStoreOptions : ObservableObject, IOptions<DbStoreOptions>
{
    private readonly IServiceProvider serviceProvider;

    /// <inheritdoc/>
    public DbStoreOptions Value { get => this; }

    /// <summary>
    /// 从数据库中获取字符串数据
    /// </summary>
    /// <param name="storage">存储字段</param>
    /// <param name="key">键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>值</returns>
    protected string GetOption(ref string? storage, string key, string defaultValue = "")
    {
        if (storage is not null)
        {
            return storage;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            storage = appDbContext.Settings.SingleOrDefault(e => e.Key == key)?.Value ?? defaultValue;
        }

        return storage;
    }

    /// <summary>
    /// 从数据库中获取bool数据
    /// </summary>
    /// <param name="storage">存储字段</param>
    /// <param name="key">键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>值</returns>
    protected bool GetOption(ref bool? storage, string key, bool defaultValue = false)
    {
        if (storage is not null)
        {
            return storage.Value;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == key)?.Value;
            storage = value is null ? defaultValue : bool.Parse(value);
        }

        return storage.Value;
    }

    /// <summary>
    /// 从数据库中获取int数据
    /// </summary>
    /// <param name="storage">存储字段</param>
    /// <param name="key">键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>值</returns>
    protected int GetOption(ref int? storage, string key, int defaultValue = 0)
    {
        if (storage is not null)
        {
            return storage.Value;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == key)?.Value;
            storage = value is null ? defaultValue : int.Parse(value, CultureInfo.InvariantCulture);
        }

        return storage.Value;
    }

    /// <summary>
    /// 从数据库中获取任何类型的数据
    /// </summary>
    /// <typeparam name="T">数据的类型</typeparam>
    /// <param name="storage">存储字段</param>
    /// <param name="key">键</param>
    /// <param name="deserializer">反序列化器</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>值</returns>
    protected T GetOption<T>(ref T? storage, string key, Func<string, T> deserializer, T defaultValue)
        where T : class
    {
        if (storage is not null)
        {
            return storage;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == key)?.Value;
            storage = value is null ? defaultValue : deserializer(value);
        }

        return storage;
    }

    /// <summary>
    /// 从数据库中获取任何类型的数据
    /// </summary>
    /// <typeparam name="T">数据的类型</typeparam>
    /// <param name="storage">存储字段</param>
    /// <param name="key">键</param>
    /// <param name="deserializer">反序列化器</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>值</returns>
    protected T GetOption<T>(ref T? storage, string key, Func<string, T> deserializer, T defaultValue)
        where T : struct
    {
        if (storage is not null)
        {
            return storage.Value;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            string? value = appDbContext.Settings.SingleOrDefault(e => e.Key == key)?.Value;
            storage = value is null ? defaultValue : deserializer(value);
        }

        return storage.Value;
    }

    /// <summary>
    /// 将值存入数据库
    /// </summary>
    /// <param name="storage">存储字段</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="propertyName">属性名称</param>
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

    /// <summary>
    /// 将值存入数据库
    /// </summary>
    /// <param name="storage">存储字段</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="propertyName">属性名称</param>
    /// <returns>是否设置了值</returns>
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

    /// <summary>
    /// 将值存入数据库
    /// </summary>
    /// <param name="storage">存储字段</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="propertyName">属性名称</param>
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

    /// <summary>
    /// 将值存入数据库
    /// </summary>
    /// <typeparam name="T">数据的类型</typeparam>
    /// <param name="storage">存储字段</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="serializer">序列化器</param>
    /// <param name="propertyName">属性名称</param>
    protected void SetOption<T>(ref T? storage, string key, T value, Func<T, string> serializer, [CallerMemberName] string? propertyName = null)
        where T : class
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

    /// <summary>
    /// 将值存入数据库
    /// </summary>
    /// <typeparam name="T">数据的类型</typeparam>
    /// <param name="storage">存储字段</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="serializer">序列化器</param>
    /// <param name="propertyName">属性名称</param>
    protected void SetOption<T>(ref T? storage, string key, T value, Func<T, string> serializer, [CallerMemberName] string? propertyName = null)
        where T : struct
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