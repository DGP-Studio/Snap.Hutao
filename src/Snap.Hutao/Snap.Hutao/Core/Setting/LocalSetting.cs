// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Factory.Process;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
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

        try
        {
            Container.Values[key] = value;
        }
        catch (Exception ex)
        {
            // 状态管理器无法写入设置
            if (HutaoNative.IsWin32(ex.HResult, WIN32_ERROR.ERROR_STATE_WRITE_SETTING_FAILED))
            {
                HutaoNative.Instance.ShowErrorMessage(ex.Message, ExceptionFormat.Format(ex));
                ProcessFactory.KillCurrent();
            }

            throw;
        }
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

    public static T Update<T>(string key, T defaultValue, Func<T, T> modifier)
    {
        Debug.Assert(SupportedTypes.Contains(typeof(T)));
        T oldValue = Get(key, defaultValue);
        Set(key, modifier(oldValue));
        return oldValue;
    }

    public static T Update<T>(string key, T defaultValue, T newValue)
    {
        Debug.Assert(SupportedTypes.Contains(typeof(T)));
        T oldValue = Get(key, defaultValue);
        Set(key, newValue);
        return oldValue;
    }
}