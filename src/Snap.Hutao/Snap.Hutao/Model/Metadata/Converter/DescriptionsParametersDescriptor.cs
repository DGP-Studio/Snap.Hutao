// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.UI.Xaml.Data.Converter;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Model.Metadata.Converter;

internal sealed partial class DescriptionsParametersDescriptor : ValueConverter<DescriptionsParameters, IList<LevelParameters<string, ParameterDescription>>>
{
    public static LevelParameters<string, ParameterDescription> Convert(DescriptionsParameters from, uint level)
    {
        LevelParameters<SkillLevel, float> param = from.Parameters.Single(param => param.Level == level);
        return new LevelParameters<string, ParameterDescription>(LevelFormat.Format(param.Level), GetParameterDescription(from.Descriptions, param.Parameters));
    }

    public override List<LevelParameters<string, ParameterDescription>> Convert(DescriptionsParameters from)
    {
        List<LevelParameters<string, ParameterDescription>> parameters = from.Parameters
            .SelectList(param => new LevelParameters<string, ParameterDescription>(LevelFormat.Format(param.Level), GetParameterDescription(from.Descriptions, param.Parameters)));

        return parameters;
    }

    [GeneratedRegex("{param([1-9][0-9]*?):(.+?)}")]
    private static partial Regex ParamRegex();

    private static List<ParameterDescription> GetParameterDescription(List<string> descriptions, List<float> paramList)
    {
        Span<string> span = CollectionsMarshal.AsSpan(descriptions);
        List<ParameterDescription> results = new(span.Length);

        foreach (string desc in span)
        {
            if (desc.AsSpan().TrySplitIntoTwo('|', out ReadOnlySpan<char> description, out ReadOnlySpan<char> format))
            {
                if (description[0] is not '#')
                {
                    // Fast path
                    string resultFormatted = ParamRegex().Replace(format.ToString(), match => ReplaceParamInMatch(match, paramList));
                    results.Add(new ParameterDescription { Description = description.ToString(), Parameter = resultFormatted });
                }
                else
                {
                    string descriptionString = SpecialNameHandler.Handle(description.ToString());
                    string formatString = SpecialNameHandler.Handle(format.ToString());

                    string resultFormatted = ParamRegex().Replace(formatString, match => ReplaceParamInMatch(match, paramList));
                    results.Add(new ParameterDescription { Description = descriptionString, Parameter = resultFormatted });
                }
            }
            else
            {
                HutaoException.InvalidOperation($"ParameterFormat failed, value: `{desc}`");
            }
        }

        return results;
    }

    private static string ReplaceParamInMatch(Match match, List<float> paramList)
    {
        if (match.Success)
        {
            int index = int.Parse(match.Groups[1].Value, CultureInfo.CurrentCulture) - 1;
            return ParameterFormat.FormatInvariant($"{{0:{match.Groups[2].Value}}}", paramList[index]);
        }

        return string.Empty;
    }
}