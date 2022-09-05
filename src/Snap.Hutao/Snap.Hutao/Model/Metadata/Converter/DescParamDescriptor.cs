// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Data;
using Snap.Hutao.Model.Metadata.Avatar;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Model.Metadata.Converter;

/// <summary>
/// 描述参数解析器
/// </summary>
internal class DescParamDescriptor : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        DescParam rawDescParam = (DescParam)value;

        // Spilt rawDesc into two parts: desc and format
        IList<DescFormat> parsedDescriptions = rawDescParam.Descriptions
            .Select(desc =>
            {
                string[] parts = desc.Split('|', 2);
                return new DescFormat(parts[0], parts[1]);
            })
            .ToList();

        IList<LevelParam<string, ParameterInfo>> parameters = rawDescParam.Parameters
            .Select(param =>
            {
                IList<ParameterInfo> parameters = GetFormattedParameters(parsedDescriptions, param.Parameters);
                return new LevelParam<string, ParameterInfo>() { Level = param.Level.ToString(), Parameters = parameters };
            })
            .ToList();

        return parameters;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw Must.NeverHappen();
    }

    private static IList<ParameterInfo> GetFormattedParameters(IList<DescFormat> formats, IList<double> param)
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
            string[] parts = match.Value[1..^1].Split(':', 2);

            int index = int.Parse(parts[0][5..]) - 1;
            if (parts[1] == "I")
            {
                return ((int)param[index]).ToString();
            }

            if (parts[1] == "F1P")
            {
                return string.Format("{0:P1}", param[index]);
            }

            if (parts[1] == "F2P")
            {
                return string.Format("{0:P2}", param[index]);
            }

            return string.Format($"{{0:{parts[1]}}}", param[index]);
        }
        else
        {
            return string.Empty;
        }
    }

    private class DescFormat
    {
        public DescFormat(string description, string format)
        {
            Description = description;
            Format = format;
        }

        public string Description { get; set; }

        public string Format { get; set; }
    }
}
