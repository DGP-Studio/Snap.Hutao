// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.UI.Xaml.Data.Converter;
using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Model.Metadata.Converter;

internal sealed partial class DescriptionsParametersDescriptor : ValueConverter<DescriptionsParameters, IList<LevelParameters<string, ParameterDescription>>>
{
    public static LevelParameters<string, ParameterDescription> Convert(DescriptionsParameters from, uint level)
    {
        return new(LevelFormat.Format(level), GetParameterDescription(from.Descriptions, from.Parameters[level]));
    }

    public override IList<LevelParameters<string, ParameterDescription>> Convert(DescriptionsParameters from)
    {
        return from.Parameters.Convert(from.Descriptions, GetParameterDescription);
    }

    [GeneratedRegex("{param([1-9][0-9]*?):(.+?)}")]
    private static partial Regex ParamRegex();

    private static ImmutableArray<ParameterDescription> GetParameterDescription(ImmutableArray<string> descriptions, ImmutableArray<float> paramArray)
    {
        ReadOnlySpan<string> span = descriptions.AsSpan();
        ImmutableArray<ParameterDescription>.Builder results = ImmutableArray.CreateBuilder<ParameterDescription>(span.Length);

        foreach (ref readonly string desc in span)
        {
            if (desc.AsSpan().TrySplitIntoTwo('|', out ReadOnlySpan<char> description, out ReadOnlySpan<char> format))
            {
                if (description[0] is not '#')
                {
                    // Fast path
                    string resultFormatted = ParamRegex().Replace(format.ToString(), match => ReplaceParamInMatch(match, paramArray));
                    results.Add(new(resultFormatted, description.ToString()));
                }
                else
                {
                    string descriptionString = SpecialNameHandling.Handle(description.ToString());
                    string formatString = SpecialNameHandling.Handle(format.ToString());

                    string resultFormatted = ParamRegex().Replace(formatString, match => ReplaceParamInMatch(match, paramArray));
                    results.Add(new(resultFormatted, descriptionString));
                }
            }
            else
            {
                HutaoException.InvalidOperation($"ParameterFormat failed, value: `{desc}`");
            }
        }

        return results.ToImmutable();
    }

    private static string ReplaceParamInMatch(Match match, ImmutableArray<float> paramArray)
    {
        if (match.Success)
        {
            int index = int.Parse(match.Groups[1].Value, CultureInfo.CurrentCulture) - 1;
            return ParameterFormat.FormatInvariant($"{{0:{match.Groups[2].Value}}}", paramArray[index]);
        }

        return string.Empty;
    }
}