// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Annotation;
using Snap.Hutao.Core.Json.Converter;
using System.Reflection;
using System.Text.Json.Serialization.Metadata;

namespace Snap.Hutao.Core.Json;

/// <summary>
/// Json 类型信息解析器
/// </summary>
internal static class JsonTypeInfoResolvers
{
    private static readonly Type JsonEnumAttributeType = typeof(JsonEnumAttribute);

    /// <summary>
    /// 解析枚举类型
    /// </summary>
    /// <param name="ti">Json 类型信息</param>
    public static void ResolveEnumType(JsonTypeInfo ti)
    {
        if (ti.Kind != JsonTypeInfoKind.Object)
        {
            return;
        }

        IEnumerable<JsonPropertyInfo> enumProperties = ti.Properties
            .Where(p => p.PropertyType.IsEnum && (p.AttributeProvider?.IsDefined(JsonEnumAttributeType, false) ?? false));

        foreach (JsonPropertyInfo enumProperty in enumProperties)
        {
            JsonEnumAttribute attr = enumProperty.AttributeProvider!.GetCustomAttributes(false).OfType<JsonEnumAttribute>().Single();

            enumProperty.CustomConverter = attr.CreateConverter(enumProperty);
        }
    }
}