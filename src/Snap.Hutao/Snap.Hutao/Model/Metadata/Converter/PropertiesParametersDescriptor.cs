﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 基础属性翻译器
/// </summary>
[HighQuality]
internal sealed class PropertiesParametersDescriptor : ValueConverter<PropertiesParameters, List<LevelParameters<string, ParameterDescription>>?>
{
    /// <inheritdoc/>
    public override List<LevelParameters<string, ParameterDescription>> Convert(PropertiesParameters from)
    {
        return from.Parameters.SelectList(param => new LevelParameters<string, ParameterDescription>()
        {
            Level = param.Level,
            Parameters = GetParameterDescriptions(param.Parameters, from.Properties),
        });
    }

    private static List<ParameterDescription> GetParameterDescriptions(List<float> parameters, List<FightProperty> properties)
    {
        List<ParameterDescription> results = new(parameters.Count);

        foreach ((float param, FightProperty property) in parameters.Zip(properties))
        {
            results.Add(FightPropertyFormat.ToParameterDescription(property, param));
        }

        return results;
    }
}