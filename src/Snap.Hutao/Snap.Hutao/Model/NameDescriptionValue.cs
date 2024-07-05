// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model;

internal sealed class NameDescriptionValue<T>
{
    public NameDescriptionValue(string name, string description, T value)
    {
        Name = name;
        Description = description;
        Value = value;
    }

    public string Name { get; }

    public string Description { get; }

    public T Value { get; }
}