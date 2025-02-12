// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text;

namespace Snap.Hutao.Core;

internal sealed class CommandLineBuilder
{
    private const char WhiteSpace = ' ';

    private readonly List<Option> options = [];

    public CommandLineBuilder AppendIf(bool condition, string name, object? value = null, CommandLineArgumentPrefix prefix = CommandLineArgumentPrefix.WhiteSpace)
    {
        return condition ? Append(name, value, prefix) : this;
    }

    public CommandLineBuilder AppendIfNotNull(string name, object? value = null)
    {
        return AppendIf(value is not null, name, value);
    }

    public CommandLineBuilder Append(string name, object? value = null, CommandLineArgumentPrefix prefix = CommandLineArgumentPrefix.WhiteSpace)
    {
        options.Add(new()
        {
            Name = name,
            Value = value?.ToString(),
            Prefix = prefix,
        });

        return this;
    }

    public override string ToString()
    {
        StringBuilder s = new();
        foreach (Option option in options)
        {
            s.Append(WhiteSpace);
            s.Append(option.Name);

            if (string.IsNullOrEmpty(option.Value))
            {
                continue;
            }

            switch (option.Prefix)
            {
                case CommandLineArgumentPrefix.WhiteSpace:
                    s.Append(' ');
                    break;
                case CommandLineArgumentPrefix.Equal:
                    s.Append('=');
                    break;
            }

            s.Append(option.Value);
        }

        return s.ToString();
    }

    private sealed class Option
    {
        public required string Name { get; init; }

        public required string? Value { get; init; }

        public required CommandLineArgumentPrefix Prefix { get; init; }
    }
}