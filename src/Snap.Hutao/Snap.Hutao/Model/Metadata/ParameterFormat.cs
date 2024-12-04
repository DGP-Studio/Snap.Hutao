// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata;

internal sealed class ParameterFormat : IFormatProvider, ICustomFormatter
{
    private static readonly Lazy<ParameterFormat> LazyFormat = new();

    public static string FormatInvariant(string str, object param)
    {
        return string.Format(LazyFormat.Value, str, param);
    }

    [SuppressMessage("", "CA1305")]
    public string Format(string? fmt, object? arg, IFormatProvider? formatProvider)
    {
        ReadOnlySpan<char> fmtSpan = fmt;
        switch (fmtSpan.Length)
        {
            case 3: // FnP
                return string.Format($"{{0:P{fmtSpan[1]}}}", arg);
            case 2: // Fn
                return string.Format($"{{0:{fmtSpan}}}", arg);
            case 1: // P I
                switch (fmtSpan[0])
                {
                    case 'P':
                        return string.Format($"{{0:P0}}", arg);
                    case 'I':
                        return arg is null ? "0" : ((IConvertible)arg).ToInt32(default).ToString();
                }

                break;
        }

        return arg?.ToString() ?? string.Empty;
    }

    public object? GetFormat(Type? formatType)
    {
        return formatType == typeof(ICustomFormatter)
            ? this
            : null;
    }
}