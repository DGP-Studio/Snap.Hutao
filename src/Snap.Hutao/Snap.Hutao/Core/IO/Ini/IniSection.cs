// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.IO.Ini;

internal sealed class IniSection : IniElement
{
    public IniSection(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public List<IniElement> Children { get; set; } = [];

    public override string ToString()
    {
        return $"[{Name}]";
    }
}