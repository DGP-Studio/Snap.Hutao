// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text;

namespace Snap.Hutao.Core;

internal sealed class CommandLineBuilder
{
    private const char WhiteSpace = ' ';

    private readonly Dictionary<string, string?> options = [];

    public CommandLineBuilder AppendIf(bool condition, string name, object? value = null)
    {
        return condition ? Append(name, value) : this;
    }

    public CommandLineBuilder AppendIfNotNull(string name, object? value = null)
    {
        return AppendIf(value is not null, name, value);
    }

    public CommandLineBuilder Append(string name, object? value = null)
    {
        options.Add(name, value?.ToString());
        return this;
    }

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