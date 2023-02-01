// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Package;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.ExceptionService;

/// <summary>
/// 帮助更好的抛出异常
/// </summary>
[System.Diagnostics.StackTraceHidden]
internal static class ThrowHelper
{
    /// <summary>
    /// 操作取消
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="inner">内部错误</param>
    /// <exception cref="OperationCanceledException">操作取消异常</exception>
    /// <returns>nothing</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static OperationCanceledException OperationCanceled(string message, Exception? inner)
    {
        throw new OperationCanceledException(message, inner);
    }

    /// <summary>
    /// 包转换错误
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="inner">内部错误</param>
    /// <returns>nothing</returns>
    /// <exception cref="PackageConvertException">包转换错误异常</exception>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static PackageConvertException PackageConvert(string message, Exception inner)
    {
        throw new PackageConvertException(message, inner);
    }

    /// <summary>
    /// 用户数据损坏
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="inner">内部错误</param>
    /// <exception cref="UserdataCorruptedException">数据损坏</exception>
    /// <returns>nothing</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static UserdataCorruptedException UserdataCorrupted(string message, Exception inner)
    {
        throw new UserdataCorruptedException(message, inner);
    }
}