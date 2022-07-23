// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Data;
using Snap.Hutao.Model.Metadata.Avatar;
using System.Collections.Generic;
using System.Linq;
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
        DescParam descParam = (DescParam)value;
        IEnumerable<DescFormat> parsedDescriptions = descParam.Descriptions.Select(desc =>
        {
            string[] parts = desc.Split('|', 2);
            return new DescFormat(parts[0], parts[1]);
        });

        IList<IList<string>> parameters = descParam.Parameters
            .Select(param =>
            {
                IList<string> parameters = GetFormattedParameters(parsedDescriptions, param.Parameters);
                parameters.Insert(0, param.Level.ToString());
                return parameters;
            })
            .ToList();

        List<string> descList = parsedDescriptions.Select(p => p.Description).ToList();
        descList.Insert(0, "等级");
        return new DescParamInternal(descList, parameters);
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw Must.NeverHappen();
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

    private IList<string> GetFormattedParameters(IEnumerable<DescFormat> formats, IList<double> param)
    {
        List<string> results = new();
        foreach (DescFormat descFormat in formats)
        {
            string format = descFormat.Format;
            string resultFormatted = Regex.Replace(format, @"{param\d+.*?}", match => EvaluateMatch(match, param));
            results.Add(resultFormatted);
        }

        return results;
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

    private class DescParamInternal
    {
        public DescParamInternal(IList<string> descriptions, IList<IList<string>> parameters)
        {
            Descriptions = descriptions;
            Parameters = parameters;
        }

        public IList<string> Descriptions { get; set; }

        public IList<IList<string>> Parameters { get; set; }
    }
}