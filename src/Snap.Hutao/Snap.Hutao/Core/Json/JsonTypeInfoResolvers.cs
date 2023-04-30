// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Annotation;
using System.Text.Json.Serialization.Metadata;

namespace Snap.Hutao.Core.Json;

/// <summary>
/// Json 类型信息解析器
/// </summary>
[HighQuality]
internal static class JsonTypeInfoResolvers
{
    private static readonly Type JsonEnumAttributeType = typeof(JsonEnumAttribute);

    /// <summary>
    /// 解析枚举类型
    /// </summary>
    /// <param name="typeInfo">Json 类型信息</param>
    public static void ResolveEnumType(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind != JsonTypeInfoKind.Object)
        {
            return;
        }

        foreach (JsonPropertyInfo property in typeInfo.Properties)
        {
            if (property.PropertyType.IsEnum)
            {
                if (property.AttributeProvider is System.Reflection.ICustomAttributeProvider provider)
                {
                    object[] attributes = provider.GetCustomAttributes(JsonEnumAttributeType, false);
                    if (attributes.SingleOrDefault() is JsonEnumAttribute attr)
                    {
                        property.CustomConverter = attr.CreateConverter(property);
                    }
                }
            }
        }
    }
}