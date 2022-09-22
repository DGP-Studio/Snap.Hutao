// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Localization;

/// <summary>
/// 翻译
/// </summary>
internal interface ITranslation
{
    /// <summary>
    /// 获取对应键的值
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>对应的值</returns>
    string this[string key] { get; }
}
