// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Data;
using Snap.Hutao.Extension;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Annotation;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 基础属性翻译器
/// </summary>
internal class PropertyInfoDescriptor : IValueConverter
{
    /// <inheritdoc/>
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is PropertyInfo rawDescParam)
        {
            IList<LevelParam<string, ParameterInfo>> parameters = rawDescParam.Parameters
            .Select(param =>
            {
                IList<ParameterInfo> parameters = GetFormattedParameters(param.Parameters, rawDescParam.Properties);
                return new LevelParam<string, ParameterInfo>() { Level = param.Level, Parameters = parameters };
            })
            .ToList();

            return parameters;
        }

        return null;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw Must.NeverHappen();
    }

    private static IList<ParameterInfo> GetFormattedParameters(IList<double> parameters, IList<FightProperty> properties)
    {
        List<ParameterInfo> results = new();

        for (int index = 0; index < parameters.Count; index++)
        {
            double param = parameters[index];
            FormatMethod method = properties[index].GetFormat();

            string valueFormatted = method switch
            {
                FormatMethod.Integer => Math.Round((double)param, MidpointRounding.AwayFromZero).ToString(),
                FormatMethod.Percent => string.Format("{0:P1}", param),
                _ => param.ToString(),
            };

            results.Add(new ParameterInfo { Description = properties[index].GetDescription(), Parameter = valueFormatted });
        }

        return results;
    }
}