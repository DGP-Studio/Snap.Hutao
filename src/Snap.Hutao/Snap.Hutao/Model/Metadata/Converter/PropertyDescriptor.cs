// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.Model.Binding.AvatarProperty;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Annotation;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 基础属性翻译器
/// </summary>
[HighQuality]
internal sealed class PropertyDescriptor : ValueConverter<PropertiesParameters, List<LevelParameters<string, ParameterDescription>>?>
{
    /// <summary>
    /// 格式化名称与值
    /// </summary>
    /// <param name="property">属性</param>
    /// <param name="value">值</param>
    /// <returns>对</returns>
    public static NameValue<string> FormatNameValue(FightProperty property, float value)
    {
        return new(property.GetLocalizedDescription(), FormatValue(property, value));
    }

    /// <summary>
    /// 格式化名称与描述
    /// </summary>
    /// <param name="property">属性</param>
    /// <param name="value">值</param>
    /// <returns>对</returns>
    public static NameDescription FormatNameDescription(FightProperty property, double value)
    {
        return new(property.GetLocalizedDescription(), FormatValue(property, value));
    }

    /// <summary>
    /// 格式化有绿字的角色属性
    /// </summary>
    /// <param name="property">战斗属性</param>
    /// <param name="name">属性名称</param>
    /// <param name="method">方法</param>
    /// <param name="baseValue">值1</param>
    /// <param name="addValue">值2</param>
    /// <returns>对2</returns>
    public static AvatarProperty FormatAvatarProperty(FightProperty property, string name, FormatMethod method, double baseValue, double addValue)
    {
        return new(property, name, FormatValue(method, baseValue + addValue), $"[{FormatValue(method, baseValue)}+{FormatValue(method, addValue)}]");
    }

    /// <summary>
    /// 格式化无绿字的角色属性
    /// </summary>
    /// <param name="property">战斗属性</param>
    /// <param name="name">属性名称</param>
    /// <param name="method">方法</param>
    /// <param name="value">值</param>
    /// <returns>对2</returns>
    public static AvatarProperty FormatAvatarProperty(FightProperty property, string name, FormatMethod method, double value)
    {
        return new(property, name, FormatValue(method, value));
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
    public override List<LevelParameters<string, ParameterDescription>> Convert(PropertiesParameters from)
    {
        return from.Parameters.SelectList(param =>
        {
            List<ParameterDescription> parameters = GetParameterInfos(param.Parameters, from.Properties);
            return new LevelParameters<string, ParameterDescription>() { Level = param.Level, Parameters = parameters };
        });
    }

    private static List<ParameterDescription> GetParameterInfos(List<double> parameters, List<FightProperty> properties)
    {
        List<ParameterDescription> results = new();

        for (int index = 0; index < parameters.Count; index++)
        {
            double param = parameters[index];
            string valueFormatted = FormatValue(properties[index], param);

            results.Add(new ParameterDescription
            {
                Description = properties[index].GetLocalizedDescription(),
                Parameter = valueFormatted,
            });
        }

        return results;
    }
}
