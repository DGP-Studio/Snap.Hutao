// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Annotation;

/// <summary>
/// 指示此方法为命令的调用方法
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class CommandAttribute : Attribute
{
    /// <summary>
    /// 指示此方法为命令的调用方法
    /// </summary>
    /// <param name="name">命令名称</param>
    public CommandAttribute(string name)
    {
    }

    /// <summary>
    /// 是否允许并行执行
    /// </summary>
    public bool AllowConcurrentExecutions { get; set; }
}