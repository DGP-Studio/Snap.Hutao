// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.Model.Metadata.Avatar;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 描述参数解析器
/// </summary>
[HighQuality]
internal sealed partial class DescriptionsParametersDescriptor : ValueConverter<DescriptionsParameters, IList<LevelParameters<string, ParameterDescription>>>
{
    /// <summary>
    /// 获取特定等级的解释
    /// </summary>
    /// <param name="from">源</param>
    /// <param name="level">等级</param>
    /// <returns>特定等级的解释</returns>
    public static LevelParameters<string, ParameterDescription> Convert(DescriptionsParameters from, int level)
    {
        LevelParameters<int, float> param = from.Parameters.Single(param => param.Level == level);
        return new LevelParameters<string, ParameterDescription>($"Lv.{param.Level}", GetParameterDescription(from.Descriptions, param.Parameters));
    }

    /// <inheritdoc/>
    public override List<LevelParameters<string, ParameterDescription>> Convert(DescriptionsParameters from)
    {
        List<LevelParameters<string, ParameterDescription>> parameters = from.Parameters
            .SelectList(param => new LevelParameters<string, ParameterDescription>($"Lv.{param.Level}", GetParameterDescription(from.Descriptions, param.Parameters)));

        return parameters;
    }

    [GeneratedRegex("{param[0-9]+.*?}")]
    private static partial Regex ParamRegex();

    private static List<ParameterDescription> GetParameterDescription(List<string> descriptions, List<float> param)
    {
        Span<string> span = CollectionsMarshal.AsSpan(descriptions);
        List<ParameterDescription> results = new(span.Length);

        foreach (ReadOnlySpan<char> desc in span)
        {
            int indexOfSeparator = desc.IndexOf('|');
            ReadOnlySpan<char> description = desc[..indexOfSeparator];
            ReadOnlySpan<char> format = desc[(indexOfSeparator + 1)..];

            string resultFormatted = ParamRegex().Replace(format.ToString(), match => ReplaceParamInMatch(match, param));
            results.Add(new ParameterDescription { Description = description.ToString(), Parameter = resultFormatted });
        }

        return results;
    }

    private static string ReplaceParamInMatch(Match match, List<float> param)
    {
        if (match.Success)
        {
            // remove parentheses and split by {value:format} like {param1:F}
            string[] parts = match.Value[1..^1].Split(':', 2);

            int index = int.Parse(parts[0]["param".Length..]) - 1;
            return ParameterFormat.Format($"{{0:{parts[1]}}}", param[index]);
        }
        else
        {
            return string.Empty;
        }
    }
}