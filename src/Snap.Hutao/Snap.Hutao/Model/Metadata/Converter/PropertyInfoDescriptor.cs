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
    /// 格式化对
    /// </summary>
    /// <param name="property">属性</param>
    /// <param name="value">值</param>
    /// <returns>对</returns>
    public static Pair<string, string> FormatPair(FightProperty property, double value)
    {
        return new(property.GetDescription(), FormatValue(property, value));
    }

    /// <summary>
    /// 格式化 对2
    /// </summary>
    /// <param name="name">属性名称</param>
    /// <param name="method">方法</param>
    /// <param name="baseValue">值1</param>
    /// <param name="addValue">值2</param>
    /// <returns>对2</returns>
    public static Pair2<string, string, string?> FormatIntegerPair2(string name, FormatMethod method, double baseValue, double addValue)
    {
        return new(name, FormatValue(method, baseValue + addValue), $"[{FormatValue(method, baseValue)}+{FormatValue(method, addValue)}]");
    }

    /// <summary>
    /// 格式化 对2
    /// </summary>
    /// <param name="name">属性名称</param>
    /// <param name="method">方法</param>
    /// <param name="value">值</param>
    /// <returns>对2</returns>
    public static Pair2<string, string, string?> FormatIntegerPair2(string name, FormatMethod method, double value)
    {
        return new(name, FormatValue(method, value), null);
    }

    /// <summary>
    /// 格式化战斗属性
    /// </summary>
    /// <param name="property">战斗属性</param>
    /// <param name="value">值</param>
    /// <returns>格式化的值</returns>
    public static string FormatValue(FightProperty property, double value)
    {
        return FormatValue(property.GetFormatMethod(), value);
    }

    /// <summary>
    /// 格式化战斗属性
    /// </summary>
    /// <param name="method">格式化方法</param>
    /// <param name="value">值</param>
    /// <returns>格式化的值</returns>
    public static string FormatValue(FormatMethod method, double value)
    {
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
            string valueFormatted = FormatValue(properties[index], param);

            results.Add(new ParameterInfo { Description = properties[index].GetDescription(), Parameter = valueFormatted });
        }

        return results;
    }
}
