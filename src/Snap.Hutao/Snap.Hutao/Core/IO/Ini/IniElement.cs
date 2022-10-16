// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.IO.Ini;

/// <summary>
/// Ini 元素
/// </summary>
internal abstract class IniElement
{
    /// <summary>
    /// 将当前元素转换到等价的字符串表示
    /// </summary>
    /// <returns>字符串</returns>
    public new abstract string ToString();
}