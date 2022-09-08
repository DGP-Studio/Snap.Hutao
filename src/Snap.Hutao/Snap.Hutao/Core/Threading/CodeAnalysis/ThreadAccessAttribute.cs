// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading.CodeAnalysis;

/// <summary>
/// 在复杂的异步方法环境下
/// 指示方法的线程访问状态
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
internal class ThreadAccessAttribute : Attribute
{
    /// <summary>
    /// 指示方法的进入线程访问状态
    /// </summary>
    /// <param name="enter">进入状态</param>
    public ThreadAccessAttribute(ThreadAccessState enter)
    {
    }

    /// <summary>
    /// 指示方法的进入退出线程访问状态
    /// </summary>
    /// <param name="enter">进入状态</param>
    /// <param name="leave">离开状态</param>
    public ThreadAccessAttribute(ThreadAccessState enter, ThreadAccessState leave)
    {
    }
}