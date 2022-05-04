// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Markup;

namespace Snap.Hutao.View.Converter;

/// <summary>
/// Static class used to provide internal tools
/// </summary>
internal static class ConvertHelper
{
    /// <summary>
    /// Helper method to safely cast an object to a boolean
    /// </summary>
    /// <param name="parameter">Parameter to cast to a boolean</param>
    /// <returns>Bool value or false if cast failed</returns>
    internal static bool TryParseBool(object parameter)
    {
        bool parsed = false;
        if (parameter != null)
        {
            _ = bool.TryParse(parameter.ToString(), out parsed);
        }

        return parsed;
    }

    /// <summary>
    /// Helper method to convert a value from a source type to a target type.
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <param name="targetType">The target type</param>
    /// <returns>The converted value</returns>
    internal static object Convert(object value, Type targetType)
    {
        return targetType.IsInstanceOfType(value)
            ? value
            : XamlBindingHelper.ConvertValue(targetType, value);
    }
}
