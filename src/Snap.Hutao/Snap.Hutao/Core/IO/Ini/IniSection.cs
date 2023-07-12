// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.IO.Ini;

/// <summary>
/// Ini 节
/// </summary>
[HighQuality]
internal sealed class IniSection : IniElement
{
    /// <summary>
    /// 构造一个新的Ini 节
    /// </summary>
    /// <param name="name">名称</param>
    public IniSection(string name)
    {
        Name = name;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"[{Name}]";
    }
}