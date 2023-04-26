// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Abstraction;

/// <summary>
/// 有名称的对象
/// 指示该对象可通过名称区分
/// </summary>
[HighQuality]
internal interface INamedService
{
    /// <summary>
    /// 名称
    /// </summary>
    string Name { get; }
}