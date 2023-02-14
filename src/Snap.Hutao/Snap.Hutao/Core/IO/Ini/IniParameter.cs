// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.IO.Ini;

/// <summary>
/// Ini 参数
/// </summary>
[HighQuality]
internal sealed class IniParameter : IniElement
{
    /// <summary>
    /// Ini 参数
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public IniParameter(string key, string value)
    {
        Key = key;
        Value = value;
    }

    /// <summary>
    /// 键
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public string Value { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Key}={Value}";
    }
}
