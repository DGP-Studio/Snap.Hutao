// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Model.Metadata;

/// <summary>
/// 参数格式化器
/// </summary>
[HighQuality]
internal sealed class ParameterFormat : IFormatProvider, ICustomFormatter
{
    /// <summary>
    /// 格式化
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="param">参数</param>
    /// <returns>格式化的字符串</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format(string str, object param)
    {
        return string.Format(new ParameterFormat(), str, param);
    }

    /// <inheritdoc/>
    public string Format(string? fmt, object? arg, IFormatProvider? formatProvider)
    {
        if (fmt != null)
        {
            switch (fmt.Length)
            {
                case 3: // FnP
                    return string.Format($"{{0:P{fmt[1]}}}", arg);
                case 2: // Fn
                    return string.Format($"{{0:{fmt}}}", arg);
                case 1: // P I
                    switch (fmt[0])
                    {
                        case 'P':
                            return string.Format($"{{0:P0}}", arg);
                        case 'I':
                            return arg == null ? "0" : ((IConvertible)arg).ToInt32(null).ToString();
                    }

                    break;
            }
        }

        return arg?.ToString() ?? string.Empty;
    }

    /// <inheritdoc/>
    public object? GetFormat(Type? formatType)
    {
        return formatType == typeof(ICustomFormatter)
            ? this
            : null;
    }
}