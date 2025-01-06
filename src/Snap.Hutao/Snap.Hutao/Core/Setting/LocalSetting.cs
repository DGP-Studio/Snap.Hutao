// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Frozen;
using System.Diagnostics;
using Windows.Storage;

namespace Snap.Hutao.Core.Setting;

internal static class LocalSetting
{
    private static readonly FrozenSet<Type> SupportedTypes =
    [
        typeof(byte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(float),
        typeof(double),
        typeof(bool),
        typeof(char),
        typeof(string),
        typeof(DateTimeOffset),
        typeof(TimeSpan),
        typeof(Guid),
        typeof(Windows.Foundation.Point),
        typeof(Windows.Foundation.Size),
        typeof(Windows.Foundation.Rect),
        typeof(ApplicationDataCompositeValue)
    ];

    private static readonly ApplicationDataContainer Container = ApplicationData.Current.LocalSettings;

    public static T Get<T>(string key, T defaultValue = default!)
    {
        Debug.Assert(SupportedTypes.Contains(typeof(T)));
        if (Container.Values.TryGetValue(key, out object? value))
        {
            // unbox the value
            return value is null ? defaultValue : (T)value;
        }

        Set(key, defaultValue);
        return defaultValue;
    }

    public static void Set<T>(string key, T value)
    {
        Debug.Assert(SupportedTypes.Contains(typeof(T)));
        Container.Values[key] = value;
    }

    public static void SetIf<T>(bool condition, string key, T value)
    {
        if (condition)
        {
            Set(key, value);
        }
    }

    public static void SetIfNot<T>(bool condition, string key, T value)
    {
        if (!condition)
        {
            Set(key, value);
        }
    }

    public static void Update<T>(string key, T defaultValue, Func<T, T> modifier)
    {
        Debug.Assert(SupportedTypes.Contains(typeof(T)));
        Set(key, modifier(Get(key, defaultValue)));
    }
}