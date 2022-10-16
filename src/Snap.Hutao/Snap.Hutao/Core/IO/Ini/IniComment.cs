// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.IO.Ini;

/// <summary>
/// Ini 注释
/// </summary>
internal class IniComment : IniElement
{
    /// <summary>
    /// 构造一个新的 Ini 注释
    /// </summary>
    /// <param name="comment">注释</param>
    public IniComment(string comment)
    {
        Comment = comment;
    }

    /// <summary>
    /// 注释
    /// </summary>
    public string Comment { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $";{Comment}";
    }
}