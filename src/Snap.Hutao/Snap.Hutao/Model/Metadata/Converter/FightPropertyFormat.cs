// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Intrinsic.Format;
using Snap.Hutao.ViewModel.AvatarProperty;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

namespace Snap.Hutao.Model.Metadata.Converter;

internal static class FightPropertyFormat
{
    public static NameValue<string> ToNameValue(FightProperty property, float value)
    {
        return new(property.GetLocalizedDescription(SH.ResourceManager) ?? string.Empty, FormatValue(property, value));
    }

    public static NameValue<string> ToNameValue(ReliquaryProperty baseProperty)
    {
        return new(baseProperty.PropertyType.GetLocalizedDescription(SH.ResourceManager) ?? string.Empty, baseProperty.Value);
    }

    public static NameStringValue ToNameStringValue(FightProperty property, float value)
    {
        return new(property.GetLocalizedDescription(SH.ResourceManager) ?? string.Empty, FormatValue(property, value));
    }

    public static NameDescription ToNameDescription(FightProperty property, float value)
    {
        return new(property.GetLocalizedDescription(SH.ResourceManager) ?? string.Empty, FormatValue(property, value));
    }

    public static ParameterDescription ToParameterDescription(FightProperty property, float value)
    {
        return new(FormatValue(property, value), property.GetLocalizedDescription(SH.ResourceManager) ?? string.Empty);
    }

    public static AvatarProperty ToAvatarProperty(BaseProperty baseProperty)
    {
        FightProperty property = baseProperty.PropertyType;
        string name = property.GetLocalizedDescription(SH.ResourceManager) ?? string.Empty;

        return new(baseProperty.PropertyType, name, baseProperty.Base, baseProperty.Add);
    }

    public static AvatarProperty ToAvatarProperty(FightProperty property, float value)
    {
        string name = property.GetLocalizedDescription(SH.ResourceManager) ?? string.Empty;
        FormatMethod method = property.GetFormatMethod();

        return new(property, name, FormatValue(method, value));
    }

    public static AvatarProperty ToAvatarProperty(FightProperty property, Dictionary<FightProperty, float> fightPropMap)
    {
        string name = property.GetLocalizedDescription(SH.ResourceManager) ?? string.Empty;
        FormatMethod method = property.GetFormatMethod();

        float value = fightPropMap.GetValueOrDefault(property);

        return new(property, name, FormatValue(method, value));
    }

    public static AvatarProperty ToAvatarProperty(FightProperty property, float baseValue, float addValue)
    {
        string name = property.GetLocalizedDescription(SH.ResourceManager) ?? string.Empty;
        FormatMethod method = property.GetFormatMethod();

        string value = FormatValue(method, baseValue + addValue);
        string addedValue = $"[{FormatValue(method, baseValue)}+{FormatValue(method, addValue)}]";

        return new(property, name, value, addedValue);
    }

    public static string FormatValue(FightProperty property, float value)
    {
        return FormatValue(property.GetFormatMethod(), value);
    }

    public static string FormatValue(FormatMethod method, float value)
    {
        return method switch
        {
            FormatMethod.Integer => $"{MathF.Round(value, MidpointRounding.AwayFromZero)}",
            FormatMethod.Percent => $"{value:P1}",
            _ => $"{value}",
        };
    }
}