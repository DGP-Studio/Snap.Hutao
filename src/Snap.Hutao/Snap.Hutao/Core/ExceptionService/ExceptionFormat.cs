// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections;
using System.Globalization;
using System.Text;

namespace Snap.Hutao.Core.ExceptionService;

internal static class ExceptionFormat
{
    private const string SectionSeparator = "----------------------------------------";

    public static string Format(Exception exception)
    {
        return Format(new(), exception).ToString();
    }

    public static StringBuilder Format(StringBuilder builder, Exception exception)
    {
        if (exception.Data.Count > 0)
        {
            builder.AppendLine("Exception Data:");

            foreach (DictionaryEntry entry in exception.Data)
            {
                builder.AppendLine(CultureInfo.CurrentCulture, $"[{TypeNameHelper.GetTypeDisplayName(entry.Value)}] {entry.Key}:'{entry.Value}'");
            }

            builder.AppendLine(SectionSeparator);
        }

        builder.Append(exception);
        return builder;
    }
}