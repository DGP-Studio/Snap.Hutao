// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Resource.Localization;

/// <summary>
/// 本地化键
/// </summary>
[HighQuality]
[AttributeUsage(AttributeTargets.Field)]
internal sealed class LocalizationKeyAttribute : Attribute
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