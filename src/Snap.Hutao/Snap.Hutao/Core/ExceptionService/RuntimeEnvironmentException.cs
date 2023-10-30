// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.ExceptionService;

/// <summary>
/// 运行环境异常
/// 用户的计算机中的某些设置不符合要求
/// </summary>
[HighQuality]
internal sealed class RuntimeEnvironmentException : Exception
{
    /// <summary>
    /// 构造一个新的运行环境异常
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="innerException">内部错误</param>
    public RuntimeEnvironmentException(string message, Exception? innerException)
        : base($"{message}\n{innerException?.Message}", innerException)
    {
    }
}