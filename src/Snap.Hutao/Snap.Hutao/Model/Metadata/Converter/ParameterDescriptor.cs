// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.Model.Metadata.Avatar;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 描述参数解析器
/// </summary>
[HighQuality]
internal sealed partial class ParameterDescriptor : ValueConverter<DescriptionsParameters, IList<LevelParameters<string, ParameterDescription>>>
{
    /// <summary>
    /// 获取特定等级的解释
    /// </summary>
    /// <param name="from">源</param>
    /// <param name="level">等级</param>
    /// <returns>特定等级的解释</returns>
    public static LevelParameters<string, ParameterDescription> Convert(DescriptionsParameters from, int level)
    {
        // DO NOT INLINE!
        // Cache the formats
        List<DescFormat> formats = from.Descriptions
            .Select(desc => new DescFormat(desc))
            .ToList();

        LevelParameters<int, double> param = from.Parameters.Single(param => param.Level == level);

        return new LevelParameters<string, ParameterDescription>($"Lv.{param.Level}", GetParameterInfos(formats, param.Parameters));
    }

    /// <inheritdoc/>
    public override List<LevelParameters<string, ParameterDescription>> Convert(DescriptionsParameters from)
    {
        // DO NOT INLINE!
        // Cache the formats
        List<DescFormat> formats = from.Descriptions
            .SelectList(desc => new DescFormat(desc));

        List<LevelParameters<string, ParameterDescription>> parameters = from.Parameters
            .SelectList(param => new LevelParameters<string, ParameterDescription>(param.Level.ToString(), GetParameterInfos(formats, param.Parameters)));

        return parameters;
    }

    private static List<ParameterDescription> GetParameterInfos(List<DescFormat> formats, List<double> param)
    {
        Span<DescFormat> span = CollectionsMarshal.AsSpan(formats);
        List<ParameterDescription> results = new(span.Length);

        foreach (DescFormat descFormat in span)
        {
            string format = descFormat.Format;
            string resultFormatted = ParamRegex().Replace(format, match => EvaluateMatch(match, param));
            results.Add(new ParameterDescription { Description = descFormat.Description, Parameter = resultFormatted });
        }

        return results;
    }

    [GeneratedRegex("{param[0-9]+.*?}")]
    private static partial Regex ParamRegex();

    private static string EvaluateMatch(Match match, IList<double> param)
    {
        if (match.Success)
        {
            // remove parentheses and split by {value:format}
            string[] parts = match.Value[1..^1].Split(':', 2);

            int index = int.Parse(parts[0]["param".Length..]) - 1;

            return ParameterFormat.Format($"{{0:{parts[1]}}}", param[index]);
        }
        else
        {
            return string.Empty;
        }
    }

    private sealed class DescFormat
    {
        public DescFormat(string desc)
        {
            // Spilt rawDesc into two parts: desc and format
            string[] parts = desc.Split('|', 2);

            Description = parts[0];
            Format = parts[1];
        }

        public string Description { get; set; }

        public string Format { get; set; }
    }
}