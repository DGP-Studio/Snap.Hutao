﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Annotation;
using System.Text.Json.Serialization.Metadata;

namespace Snap.Hutao.Core.Json;

internal static class JsonTypeInfoResolvers
{
    private static readonly Type JsonEnumAttributeType = typeof(JsonEnumAttribute);

    public static void ResolveEnumType(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind != JsonTypeInfoKind.Object)
        {
            return;
        }

        foreach (JsonPropertyInfo property in typeInfo.Properties)
        {
            if (!property.PropertyType.IsEnum)
            {
                continue;
            }

            if (property.AttributeProvider is not { } provider)
            {
                continue;
            }

            object[] attributes = provider.GetCustomAttributes(JsonEnumAttributeType, false);
            if (attributes.SingleOrDefault() is JsonEnumAttribute attr)
            {
                property.CustomConverter = attr.CreateConverter(property);
            }
        }
    }
}