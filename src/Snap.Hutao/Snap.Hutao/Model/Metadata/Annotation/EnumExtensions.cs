// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Reflection;

namespace Snap.Hutao.Model.Metadata.Annotation;

/// <summary>
/// 枚举拓展
/// </summary>
internal static class EnumExtensions
{
    /// <summary>
    /// 获取枚举的描述
    /// </summary>
    /// <typeparam name="TEnum">枚举的类型</typeparam>
    /// <param name="enum">枚举值</param>
    /// <returns>描述</returns>
    internal static FormatMethod GetFormatMethod<TEnum>(this TEnum @enum)
        where TEnum : struct, Enum
    {
        string enumName = Must.NotNull(Enum.GetName(@enum)!);
        FieldInfo? field = @enum.GetType().GetField(enumName);
        FormatAttribute? attr = field?.GetCustomAttribute<FormatAttribute>();
        return attr?.Method ?? FormatMethod.None;
    }
}