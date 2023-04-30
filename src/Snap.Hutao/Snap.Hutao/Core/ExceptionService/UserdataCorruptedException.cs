// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.ExceptionService;

/// <summary>
/// 用户数据损坏异常
/// </summary>
[HighQuality]
internal sealed class UserdataCorruptedException : Exception
{
    /// <summary>
    /// 构造一个新的用户数据损坏异常
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="innerException">内部错误</param>
    public UserdataCorruptedException(string message, Exception innerException)
        : base(string.Format(SH.CoreExceptionServiceUserdataCorruptedMessage, $"{message}\n{innerException.Message}"), innerException)
    {
    }
}