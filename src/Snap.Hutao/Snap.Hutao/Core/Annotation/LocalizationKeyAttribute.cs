// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Annotation;

/// <summary>
/// 本地化键
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
internal class LocalizationKeyAttribute : Attribute
{
    /// <summary>
    /// 指定本地化键
    /// </summary>
    /// <param name="key">键</param>
    public LocalizationKeyAttribute(string key)
    {
        Key = key;
    }

    /// <summary>
    /// 键
    /// </summary>
    public string Key { get; }
}
