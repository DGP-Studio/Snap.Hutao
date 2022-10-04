// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.Extension;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Annotation;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 基础属性翻译器
/// </summary>
internal class PropertyInfoDescriptor : ValueConverterBase<PropertyInfo, IList<LevelParam<string, ParameterInfo>>?>
{
    /// <summary>
    /// 格式化战斗属性
    /// </summary>
    /// <param name="property">战斗属性</param>
    /// <param name="value">值</param>
    /// <returns>格式化的值</returns>
    public static string FormatProperty(FightProperty property, double value)
    {
        FormatMethod method = property.GetFormatMethod();

        string valueFormatted = method switch
        {
            FormatMethod.Integer => Math.Round((double)value, MidpointRounding.AwayFromZero).ToString(),
            FormatMethod.Percent => string.Format("{0:P1}", value),
            _ => value.ToString(),
        };
        return valueFormatted;
    }

    /// <inheritdoc/>
    public override IList<LevelParam<string, ParameterInfo>> Convert(PropertyInfo from)
    {
        IList<LevelParam<string, ParameterInfo>> parameters = from.Parameters
            .Select(param =>
            {
                IList<ParameterInfo> parameters = GetParameterInfos(param.Parameters, from.Properties);
                return new LevelParam<string, ParameterInfo>() { Level = param.Level, Parameters = parameters };
            })
            .ToList();

        return parameters;
    }

    private static IList<ParameterInfo> GetParameterInfos(IList<double> parameters, IList<FightProperty> properties)
    {
        List<ParameterInfo> results = new();

        for (int index = 0; index < parameters.Count; index++)
        {
            double param = parameters[index];
            string valueFormatted = FormatProperty(properties[index], param);

            results.Add(new ParameterInfo { Description = properties[index].GetDescription(), Parameter = valueFormatted });
        }

        return results;
    }
}