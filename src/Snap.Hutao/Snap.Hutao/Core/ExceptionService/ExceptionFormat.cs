// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections;
using System.Globalization;
using System.Text;

namespace Snap.Hutao.Core.ExceptionService;

internal sealed class ExceptionFormat
{
    private const string SectionSeparator = "----------------------------------------";

    public static string Format(Exception exception)
    {
        StringBuilder builder = new();
        Format(builder, exception);
        return builder.ToString();
    }

    public static void Format(StringBuilder builder, Exception exception)
    {
        builder.AppendLine("Exception Data:");

        foreach (DictionaryEntry entry in exception.Data)
        {
            builder.AppendLine(CultureInfo.CurrentCulture, $"[{TypeNameHelper.GetTypeDisplayName(entry.Value)}]:{entry.Key}:{entry.Value}");
        }

        builder.AppendLine(SectionSeparator);
        builder.Append(exception);
    }
}