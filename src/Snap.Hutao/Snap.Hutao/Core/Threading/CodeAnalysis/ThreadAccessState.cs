// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading.CodeAnalysis;

/// <summary>
/// 线程访问情况
/// </summary>
internal enum ThreadAccessState
{
    /// <summary>
    /// 任何线程均有可能访问该方法
    /// </summary>
    AnyThread,

    /// <summary>
    /// 仅主线程有机会访问该方法
    /// 仅允许主线程访问该方法
    /// </summary>
    MainThread,
}