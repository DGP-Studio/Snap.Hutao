// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Annotation;
using System.Reflection;

namespace Snap.Hutao.Extension;

/// <summary>
/// 枚举拓展
/// </summary>
public static class EnumExtension
{
    /// <summary>
    /// 获取枚举的描述
    /// </summary>
    /// <typeparam name="TEnum">枚举的类型</typeparam>
    /// <param name="enum">枚举值</param>
    /// <returns>描述</returns>
    public static string GetDescription<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        string enumName = Enum.GetName(@enum)!;
        FieldInfo? field = @enum.GetType().GetField(enumName);
        DescriptionAttribute? attr = field?.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description ?? enumName;
    }

    /// <summary>
    /// 获取枚举的描述
    /// </summary>
    /// <typeparam name="TEnum">枚举的类型</typeparam>
    /// <param name="enum">枚举值</param>
    /// <returns>描述</returns>
    public static string? GetDescriptionOrNull<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        string enumName = Enum.GetName(@enum)!;
        FieldInfo? field = @enum.GetType().GetField(enumName);
        DescriptionAttribute? attr = field?.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description;
    }

    /// <summary>
    /// 获取本地化的描述
    /// </summary>
    /// <typeparam name="TEnum">枚举的类型</typeparam>
    /// <param name="enum">枚举值</param>
    /// <returns>本地化的描述</returns>
    public static string GetLocalizedDescription<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        string enumName = Enum.GetName(@enum)!;
        FieldInfo? field = @enum.GetType().GetField(enumName);
        LocalizationKeyAttribute? attr = field?.GetCustomAttribute<LocalizationKeyAttribute>();
        string? result = null;
        if (attr != null)
        {
            result = SH.ResourceManager.GetString(attr.Key);
        }

        return result ?? enumName;
    }
}