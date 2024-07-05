// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Storage;

namespace Snap.Hutao.Core.Setting;

[HighQuality]
internal static class LocalSetting
{
    private static readonly ApplicationDataContainer Container = ApplicationData.Current.LocalSettings;

    public static byte Get(string key, byte defaultValue)
    {
        return Get<byte>(key, defaultValue);
    }

    public static short Get(string key, short defaultValue)
    {
        return Get<short>(key, defaultValue);
    }

    public static ushort Get(string key, ushort defaultValue)
    {
        return Get<ushort>(key, defaultValue);
    }

    public static int Get(string key, int defaultValue)
    {
        return Get<int>(key, defaultValue);
    }

    public static uint Get(string key, uint defaultValue)
    {
        return Get<uint>(key, defaultValue);
    }

    public static long Get(string key, long defaultValue)
    {
        return Get<long>(key, defaultValue);
    }

    public static ulong Get(string key, ulong defaultValue)
    {
        return Get<ulong>(key, defaultValue);
    }

    public static float Get(string key, float defaultValue)
    {
        return Get<float>(key, defaultValue);
    }

    public static double Get(string key, double defaultValue)
    {
        return Get<double>(key, defaultValue);
    }

    public static bool Get(string key, bool defaultValue)
    {
        return Get<bool>(key, defaultValue);
    }

    public static char Get(string key, char defaultValue)
    {
        return Get<char>(key, defaultValue);
    }

    public static string Get(string key, string defaultValue)
    {
        return Get<string>(key, defaultValue);
    }

    public static DateTimeOffset Get(string key, in DateTimeOffset defaultValue)
    {
        return Get<DateTimeOffset>(key, defaultValue);
    }

    public static TimeSpan Get(string key, in TimeSpan defaultValue)
    {
        return Get<TimeSpan>(key, defaultValue);
    }

    public static Guid Get(string key, in Guid defaultValue)
    {
        return Get<Guid>(key, defaultValue);
    }

    public static Windows.Foundation.Point Get(string key, Windows.Foundation.Point defaultValue)
    {
        return Get<Windows.Foundation.Point>(key, defaultValue);
    }

    public static Windows.Foundation.Size Get(string key, Windows.Foundation.Size defaultValue)
    {
        return Get<Windows.Foundation.Size>(key, defaultValue);
    }

    public static Windows.Foundation.Rect Get(string key, Windows.Foundation.Rect defaultValue)
    {
        return Get<Windows.Foundation.Rect>(key, defaultValue);
    }

    public static ApplicationDataCompositeValue Get(string key, ApplicationDataCompositeValue defaultValue)
    {
        return Get<ApplicationDataCompositeValue>(key, defaultValue);
    }

    public static void Set(string key, byte value)
    {
        Set<byte>(key, value);
    }

    public static void Set(string key, short value)
    {
        Set<short>(key, value);
    }

    public static void Set(string key, ushort value)
    {
        Set<ushort>(key, value);
    }

    public static void Set(string key, int value)
    {
        Set<int>(key, value);
    }

    public static void Set(string key, uint value)
    {
        Set<uint>(key, value);
    }

    public static void Set(string key, long value)
    {
        Set<long>(key, value);
    }

    public static void Set(string key, ulong value)
    {
        Set<ulong>(key, value);
    }

    public static void Set(string key, float value)
    {
        Set<float>(key, value);
    }

    public static void Set(string key, double value)
    {
        Set<double>(key, value);
    }

    public static void Set(string key, bool value)
    {
        Set<bool>(key, value);
    }

    public static void Set(string key, char value)
    {
        Set<char>(key, value);
    }

    public static void Set(string key, string value)
    {
        Set<string>(key, value);
    }

    public static void Set(string key, in DateTimeOffset value)
    {
        Set<DateTimeOffset>(key, value);
    }

    public static void Set(string key, in TimeSpan value)
    {
        Set<TimeSpan>(key, value);
    }

    public static void Set(string key, in Guid value)
    {
        Set<Guid>(key, value);
    }

    public static void Set(string key, Windows.Foundation.Point value)
    {
        Set<Windows.Foundation.Point>(key, value);
    }

    public static void Set(string key, Windows.Foundation.Size value)
    {
        Set<Windows.Foundation.Size>(key, value);
    }

    public static void Set(string key, Windows.Foundation.Rect value)
    {
        Set<Windows.Foundation.Rect>(key, value);
    }

    public static void Set(string key, ApplicationDataCompositeValue value)
    {
        Set<ApplicationDataCompositeValue>(key, value);
    }

    public static void Update(string key, int defaultValue, Func<int, int> modifier)
    {
        Set<int?>(key, modifier(Get<int>(key, defaultValue)));
    }

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

    private static void Set<T>(string key, T value)
    {
        Container.Values[key] = value;
    }
}