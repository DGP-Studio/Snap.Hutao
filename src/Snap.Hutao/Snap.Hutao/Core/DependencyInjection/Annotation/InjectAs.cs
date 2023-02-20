// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection.Annotation;

/// <summary>
/// 注入方法
/// </summary>
[HighQuality]
internal enum InjectAs
{
    /// <summary>
    /// 指示应注册为单例对象
    /// </summary>
    Singleton,

    /// <summary>
    /// 指示应注册为短期对象
    /// </summary>
    Transient,

    /// <summary>
    /// 指示应注册为范围对象
    /// </summary>
    Scoped,
}
