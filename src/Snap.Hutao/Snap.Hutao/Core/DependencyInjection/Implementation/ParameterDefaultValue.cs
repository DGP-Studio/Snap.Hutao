// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Reflection;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.DependencyInjection.Implementation;

internal static class ParameterDefaultValue
{
    public static bool TryGetDefaultValue(ParameterInfo parameter, out object? defaultValue)
    {
        bool hasDefaultValue = CheckHasDefaultValue(parameter, out bool tryToGetDefaultValue);
        defaultValue = null;

        if (hasDefaultValue)
        {
            if (tryToGetDefaultValue)
            {
                defaultValue = parameter.DefaultValue;
            }

            bool isNullableParameterType = parameter.ParameterType.IsGenericType &&
                parameter.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>);

            // Workaround for https://github.com/dotnet/runtime/issues/18599
            if (defaultValue == null && parameter.ParameterType.IsValueType && !isNullableParameterType /* Nullable types should be left null */)
            {
                defaultValue = CreateValueType(parameter.ParameterType);
            }

            [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2067:UnrecognizedReflectionPattern", Justification = "CreateValueType is only called on a ValueType. You can always create an instance of a ValueType.")]
            static object? CreateValueType(Type t)
            {
                return RuntimeHelpers.GetUninitializedObject(t);
            }

            // Handle nullable enums
            if (defaultValue != null && isNullableParameterType)
            {
                Type? underlyingType = Nullable.GetUnderlyingType(parameter.ParameterType);
                if (underlyingType is { IsEnum: true })
                {
                    defaultValue = Enum.ToObject(underlyingType, defaultValue);
                }
            }
        }

        return hasDefaultValue;
    }

    public static bool CheckHasDefaultValue(ParameterInfo parameter, out bool tryToGetDefaultValue)
    {
        tryToGetDefaultValue = true;
        return parameter.HasDefaultValue;
    }
}