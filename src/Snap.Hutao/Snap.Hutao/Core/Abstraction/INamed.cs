// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Abstraction;

/// <summary>
/// 有名称的对象
/// 指示该对象可通过名称区分
/// </summary>
internal interface INamed
{
    /// <summary>
    /// 名称
    /// </summary>
    string Name { get; }
}