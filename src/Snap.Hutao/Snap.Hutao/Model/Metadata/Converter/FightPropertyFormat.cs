// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Intrinsic.Format;
using Snap.Hutao.ViewModel.AvatarProperty;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 战斗属性格式化
/// </summary>
internal static class FightPropertyFormat
{
    /// <summary>
    /// 格式化名称与值
    /// </summary>
    /// <param name="property">属性</param>
    /// <param name="value">值</param>
    /// <returns>对</returns>
    public static NameValue<string> ToNameValue(FightProperty property, float value)
    {
        return new(property.GetLocalizedDescription(), FormatValue(property, value));
    }

    /// <summary>
    /// 格式化名称与描述
    /// </summary>
    /// <param name="property">属性</param>
    /// <param name="value">值</param>
    /// <returns>对</returns>
    public static NameDescription ToNameDescription(FightProperty property, float value)
    {
        return new(property.GetLocalizedDescription(), FormatValue(property, value));
    }

    /// <summary>
    /// 格式化有绿字的角色属性
    /// </summary>
    /// <param name="property">战斗属性</param>
    /// <param name="baseValue">白字</param>
    /// <param name="addValue">绿字</param>
    /// <returns>对2</returns>
    public static AvatarProperty ToAvatarProperty(FightProperty property, float baseValue, float addValue)
    {
        string name = property.GetLocalizedDescription();
        FormatMethod method = property.GetFormatMethod();

        string value = FormatValue(method, baseValue + addValue);
        string addedValue = $"[{FormatValue(method, baseValue)}+{FormatValue(method, addValue)}]";

        return new(property, name, value, addedValue);
    }

    /// <summary>
    /// 格式化无绿字的角色属性
    /// </summary>
    /// <param name="property">战斗属性</param>
    /// <param name="value">值</param>
    /// <returns>对2</returns>
    public static AvatarProperty ToAvatarProperty(FightProperty property, float value)
    {
        string name = property.GetLocalizedDescription();
        FormatMethod method = property.GetFormatMethod();

        return new(property, name, FormatValue(method, value));
    }

    /// <summary>
    /// 格式化无绿字的角色属性
    /// </summary>
    /// <param name="property">战斗属性</param>
    /// <param name="fightPropMap">战斗属性映射</param>
    /// <returns>对2</returns>
    public static AvatarProperty ToAvatarProperty(FightProperty property, Dictionary<FightProperty, float> fightPropMap)
    {
        string name = property.GetLocalizedDescription();
        FormatMethod method = property.GetFormatMethod();

        float value = fightPropMap.GetValueOrDefault(property);

        return new(property, name, FormatValue(method, value));
    }

    /// <summary>
    /// 格式化战斗属性
    /// </summary>
    /// <param name="property">战斗属性</param>
    /// <param name="value">值</param>
    /// <returns>格式化的值</returns>
    public static string FormatValue(FightProperty property, float value)
    {
        return FormatValue(property.GetFormatMethod(), value);
    }

    /// <summary>
    /// 格式化战斗属性
    /// </summary>
    /// <param name="method">格式化方法</param>
    /// <param name="value">值</param>
    /// <returns>格式化的值</returns>
    public static string FormatValue(FormatMethod method, float value)
    {
        return method switch
        {
            FormatMethod.Integer => $"{Math.Round((double)value, MidpointRounding.AwayFromZero)}",
            FormatMethod.Percent => string.Format("{0:P1}", value),
            _ => value.ToString(),
        };
    }
}