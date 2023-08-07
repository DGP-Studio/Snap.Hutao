// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text;

namespace Snap.Hutao.Core;

/// <summary>
/// 命令行建造器
/// </summary>
[HighQuality]
internal sealed class CommandLineBuilder
{
    private const char WhiteSpace = ' ';
    private readonly Dictionary<string, string?> options = new();

    /// <summary>
    /// 当符合条件时添加参数
    /// </summary>
    /// <param name="name">参数名称</param>
    /// <param name="condition">条件</param>
    /// <param name="value">值</param>
    /// <returns>命令行建造器</returns>
    public CommandLineBuilder AppendIf(string name, bool condition, object? value = null)
    {
        return condition ? Append(name, value) : this;
    }

    /// <summary>
    /// 当参数不为 null 时添加参数
    /// </summary>
    /// <param name="name">参数名称</param>
    /// <param name="value">值</param>
    /// <returns>命令行建造器</returns>
    public CommandLineBuilder AppendIfNotNull(string name, object? value = null)
    {
        return AppendIf(name, value is not null, value);
    }

    /// <summary>
    /// 添加参数
    /// </summary>
    /// <param name="name">参数名称</param>
    /// <param name="value">值</param>
    /// <returns>命令行建造器</returns>
    public CommandLineBuilder Append(string name, object? value = null)
    {
        options.Add(name, value?.ToString());
        return this;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        StringBuilder s = new();
        foreach ((string key, string? value) in options)
        {
            s.Append(WhiteSpace);
            s.Append(key);

            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            s.Append(WhiteSpace);
            s.Append(value);
        }

        return s.ToString();
    }
}