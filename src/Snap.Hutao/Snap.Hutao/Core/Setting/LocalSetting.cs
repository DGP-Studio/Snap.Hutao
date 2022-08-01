// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;
using Windows.Storage;

namespace Snap.Hutao.Core.Setting;

/// <summary>
/// 本地设置
/// </summary>
internal static class LocalSetting
{
    /// <summary>
    /// 由于 <see cref="Windows.Foundation.Collections.IPropertySet"/> 没有 nullable context,
    /// 在处理引用类型时需要格外小心
    /// </summary>
    private static readonly ApplicationDataContainer Container;

    static LocalSetting()
    {
        Container = App.Settings;
    }

    /// <summary>
    /// 获取设置项的值
    /// </summary>
    /// <typeparam name="T">设置项的类型</typeparam>
    /// <param name="key">键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>获取的值</returns>
    public static T? Get<T>(string key, T? defaultValue = default)
        where T : class
    {
        if (Container.Values.TryGetValue(key, out object? value))
        {
            return value is null ? defaultValue : value as T;
        }
        else
        {
            Set(key, defaultValue);
            return defaultValue;
        }
    }

    /// <summary>
    /// 获取设置项的值
    /// </summary>
    /// <typeparam name="T">设置项的类型</typeparam>
    /// <param name="key">键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>获取的值</returns>
    public static T GetValueType<T>(string key, T defaultValue = default)
        where T : struct
    {
        if (Container.Values.TryGetValue(key, out object? value))
        {
            if (value is null)
            {
                return defaultValue;
            }
            else
            {
                // 无法避免的拆箱操作
                return (T)value;
            }
        }
        else
        {
            SetValueType(key, defaultValue);
            return defaultValue;
        }
    }

    /// <summary>
    /// 设置设置项的值
    /// </summary>
    /// <typeparam name="T">设置项的类型</typeparam>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>设置的值</returns>
    public static T Set<T>(string key, T value)
    {
        Container.Values[key] = value;
        return value;
    }

    /// <summary>
    /// 设置设置项的值
    /// </summary>
    /// <typeparam name="T">设置项的类型</typeparam>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public static void SetValueType<T>(string key, T value)
        where T : struct
    {
        try
        {
            Container.Values[key] = value;
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}
