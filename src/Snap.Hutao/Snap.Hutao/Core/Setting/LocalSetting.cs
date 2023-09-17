// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Storage;

namespace Snap.Hutao.Core.Setting;

/// <summary>
/// 本地设置
/// </summary>
[HighQuality]
internal static class LocalSetting
{
    private static readonly ApplicationDataContainer Container = ApplicationData.Current.LocalSettings;

    /// <inheritdoc cref="Get{T}(string, T)"/>
    public static byte Get(string key, byte defaultValue)
    {
        return Get<byte>(key, defaultValue);
    }

    /// <inheritdoc cref="Get{T}(string, T)"/>
    public static short Get(string key, short defaultValue)
    {
        return Get<short>(key, defaultValue);
    }

    /// <inheritdoc cref="Get{T}(string, T)"/>
    public static ushort Get(string key, ushort defaultValue)
    {
        return Get<ushort>(key, defaultValue);
    }

    /// <inheritdoc cref="Get{T}(string, T)"/>
    public static int Get(string key, int defaultValue)
    {
        return Get<int>(key, defaultValue);
    }

    /// <inheritdoc cref="Get{T}(string, T)"/>
    public static uint Get(string key, uint defaultValue)
    {
        return Get<uint>(key, defaultValue);
    }

    /// <inheritdoc cref="Get{T}(string, T)"/>
    public static ulong Get(string key, ulong defaultValue)
    {
        return Get<ulong>(key, defaultValue);
    }

    /// <inheritdoc cref="Get{T}(string, T)"/>
    public static float Get(string key, float defaultValue)
    {
        return Get<float>(key, defaultValue);
    }

    /// <inheritdoc cref="Get{T}(string, T)"/>
    public static double Get(string key, double defaultValue)
    {
        return Get<double>(key, defaultValue);
    }

    /// <inheritdoc cref="Get{T}(string, T)"/>
    public static bool Get(string key, bool defaultValue)
    {
        return Get<bool>(key, defaultValue);
    }

    /// <inheritdoc cref="Get{T}(string, T)"/>
    public static char Get(string key, char defaultValue)
    {
        return Get<char>(key, defaultValue);
    }

    /// <inheritdoc cref="Get{T}(string, T)"/>
    public static string Get(string key, string defaultValue)
    {
        return Get<string>(key, defaultValue);
    }

    /// <inheritdoc cref="Get{T}(string, T)"/>
    public static DateTimeOffset Get(string key, in DateTimeOffset defaultValue)
    {
        return Get<DateTimeOffset>(key, defaultValue);
    }

    /// <inheritdoc cref="Get{T}(string, T)"/>
    public static TimeSpan Get(string key, in TimeSpan defaultValue)
    {
        return Get<TimeSpan>(key, defaultValue);
    }

    /// <inheritdoc cref="Get{T}(string, T)"/>
    public static Guid Get(string key, in Guid defaultValue)
    {
        return Get<Guid>(key, defaultValue);
    }

    /// <inheritdoc cref="Get{T}(string, T)"/>
    public static Windows.Foundation.Point Get(string key, Windows.Foundation.Point defaultValue)
    {
        return Get<Windows.Foundation.Point>(key, defaultValue);
    }

    /// <inheritdoc cref="Get{T}(string, T)"/>
    public static Windows.Foundation.Size Get(string key, Windows.Foundation.Size defaultValue)
    {
        return Get<Windows.Foundation.Size>(key, defaultValue);
    }

    /// <inheritdoc cref="Get{T}(string, T)"/>
    public static Windows.Foundation.Rect Get(string key, Windows.Foundation.Rect defaultValue)
    {
        return Get<Windows.Foundation.Rect>(key, defaultValue);
    }

    public static ApplicationDataCompositeValue Get(string key, ApplicationDataCompositeValue defaultValue)
    {
        return Get<ApplicationDataCompositeValue>(key, defaultValue);
    }

    /// <inheritdoc cref="Set{T}(string, T)"/>
    public static void Set(string key, byte value)
    {
        Set<byte>(key, value);
    }

    /// <inheritdoc cref="Set{T}(string, T)"/>
    public static void Set(string key, short value)
    {
        Set<short>(key, value);
    }

    /// <inheritdoc cref="Set{T}(string, T)"/>
    public static void Set(string key, ushort value)
    {
        Set<ushort>(key, value);
    }

    /// <inheritdoc cref="Set{T}(string, T)"/>
    public static void Set(string key, int value)
    {
        Set<int>(key, value);
    }

    /// <inheritdoc cref="Set{T}(string, T)"/>
    public static void Set(string key, uint value)
    {
        Set<uint>(key, value);
    }

    /// <inheritdoc cref="Set{T}(string, T)"/>
    public static void Set(string key, ulong value)
    {
        Set<ulong>(key, value);
    }

    /// <inheritdoc cref="Set{T}(string, T)"/>
    public static void Set(string key, float value)
    {
        Set<float>(key, value);
    }

    /// <inheritdoc cref="Set{T}(string, T)"/>
    public static void Set(string key, double value)
    {
        Set<double>(key, value);
    }

    /// <inheritdoc cref="Set{T}(string, T)"/>
    public static void Set(string key, bool value)
    {
        Set<bool>(key, value);
    }

    /// <inheritdoc cref="Set{T}(string, T)"/>
    public static void Set(string key, char value)
    {
        Set<char>(key, value);
    }

    /// <inheritdoc cref="Set{T}(string, T)"/>
    public static void Set(string key, string value)
    {
        Set<string>(key, value);
    }

    /// <inheritdoc cref="Set{T}(string, T)"/>
    public static void Set(string key, in DateTimeOffset value)
    {
        Set<DateTimeOffset>(key, value);
    }

    /// <inheritdoc cref="Set{T}(string, T)"/>
    public static void Set(string key, in TimeSpan value)
    {
        Set<TimeSpan>(key, value);
    }

    /// <inheritdoc cref="Set{T}(string, T)"/>
    public static void Set(string key, in Guid value)
    {
        Set<Guid>(key, value);
    }

    /// <inheritdoc cref="Set{T}(string, T)"/>
    public static void Set(string key, Windows.Foundation.Point value)
    {
        Set<Windows.Foundation.Point>(key, value);
    }

    /// <inheritdoc cref="Set{T}(string, T)"/>
    public static void Set(string key, Windows.Foundation.Size value)
    {
        Set<Windows.Foundation.Size>(key, value);
    }

    /// <inheritdoc cref="Set{T}(string, T)"/>
    public static void Set(string key, Windows.Foundation.Rect value)
    {
        Set<Windows.Foundation.Rect>(key, value);
    }

    public static void Set(string key, ApplicationDataCompositeValue value)
    {
        Set<ApplicationDataCompositeValue>(key, value);
    }

    /// <summary>
    /// 获取设置项的值
    /// </summary>
    /// <typeparam name="T">设置项的类型</typeparam>
    /// <param name="key">键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>获取的值</returns>
    private static T Get<T>(string key, T defaultValue = default!)
    {
        if (Container.Values.TryGetValue(key, out object? value))
        {
            // unbox the value
            return value is null ? defaultValue : (T)value;
        }
        else
        {
            Set(key, defaultValue);
            return defaultValue;
        }
    }

    /// <summary>
    /// 设置设置项的值
    /// </summary>
    /// <typeparam name="T">设置项的类型</typeparam>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    private static void Set<T>(string key, T value)
    {
        Container.Values[key] = value;
    }
}