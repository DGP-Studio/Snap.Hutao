// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model;

/// <summary>
/// 名称与图标
/// </summary>
[HighQuality]
internal interface INameIcon
{
    /// <summary>
    /// 名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 图标
    /// </summary>
    Uri Icon { get; }
}