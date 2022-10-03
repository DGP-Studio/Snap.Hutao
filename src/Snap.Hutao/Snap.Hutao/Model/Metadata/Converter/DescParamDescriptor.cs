// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Control;
using Snap.Hutao.Model.Metadata.Avatar;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 描述参数解析器
/// </summary>
internal sealed class DescParamDescriptor : ValueConverterBase<DescParam, IList<LevelParam<string, ParameterInfo>>>
{
    /// <summary>
    /// 获取特定等级的解释
    /// </summary>
    /// <param name="from">源</param>
    /// <param name="level">等级</param>
    /// <returns>特定等级的解释</returns>
    public static LevelParam<string, ParameterInfo> Convert(DescParam from, int level)
    {
        // DO NOT INLINE!
        // Cache the formats
        IList<DescFormat> formats = from.Descriptions
            .Select(desc => new DescFormat(desc))
            .ToList();

        LevelParam<int, double> param = from.Parameters.Single(param => param.Level == level);

        return new LevelParam<string, ParameterInfo>(param.Level.ToString(), GetParameterInfos(formats, param.Parameters));
    }

    /// <inheritdoc/>
    public override IList<LevelParam<string, ParameterInfo>> Convert(DescParam from)
    {
        // DO NOT INLINE!
        // Cache the formats
        IList<DescFormat> formats = from.Descriptions
            .Select(desc => new DescFormat(desc))
            .ToList();

        IList<LevelParam<string, ParameterInfo>> parameters = from.Parameters
            .Select(param => new LevelParam<string, ParameterInfo>(param.Level.ToString(), GetParameterInfos(formats, param.Parameters)))
            .ToList();

        return parameters;
    }

    private static IList<ParameterInfo> GetParameterInfos(IList<DescFormat> formats, IList<double> param)
    {
        List<ParameterInfo> results = new();

        for (int index = 0; index < formats.Count; index++)
        {
            DescFormat descFormat = formats[index];

            string format = descFormat.Format;
            string resultFormatted = Regex.Replace(format, @"{param\d+.*?}", match => EvaluateMatch(match, param));
            results.Add(new ParameterInfo { Description = descFormat.Description, Parameter = resultFormatted });
        }

        return results;
    }

    private static string EvaluateMatch(Match match, IList<double> param)
    {
        if (match.Success)
        {
            // remove parentheses and split by {value:format}
            string[] parts = match.Value[1..^1].Split(':', 2);

            int index = int.Parse(parts[0]["param".Length..]) - 1;

            return string.Format(new ParameterFormat(), $"{{0:{parts[1]}}}", param[index]);
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