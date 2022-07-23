// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Validation;

/// <summary>
/// 封装验证方法,简化微软验证
/// </summary>
public static class Must
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the specified parameter's value is null.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="value">The value of the argument.</param>
    /// <param name="parameterName">The name of the parameter to include in any thrown exception.</param>
    /// <returns>The value of the parameter.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <c>null</c>.</exception>
    [DebuggerStepThrough]
    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    public static T NotNull<T>([NotNull] T value, [CallerArgumentExpression("value")] string? parameterName = null)
        where T : class // ensures value-types aren't passed to a null checking method
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }

        return value;
    }

    /// <summary>
    /// Unconditionally throws an <see cref="NotSupportedException"/>.
    /// </summary>
    /// <returns>Nothing. This method always throws.</returns>
    [DebuggerStepThrough]
    [DoesNotReturn]
    public static Exception NeverHappen()
    {
        // Keep these two as separate lines of code, so the debugger can come in during the assert dialog
        // that the exception's constructor displays, and the debugger can then be made to skip the throw
        // in order to continue the investigation.
        NotSupportedException exception = new("该行为不应发生，请联系开发者进一步确认");
        bool proceed = true; // allows debuggers to skip the throw statement
        if (proceed)
        {
            throw exception;
        }
        else
        {
#pragma warning disable CS8763
            return new Exception();
#pragma warning restore CS8763
        }
    }

    /// <summary>
    /// Unconditionally throws an <see cref="NotSupportedException"/>.
    /// </summary>
    /// <typeparam name="T">The type that the method should be typed to return (although it never returns anything).</typeparam>
    /// <returns>Nothing. This method always throws.</returns>
    [DebuggerStepThrough]
    [DoesNotReturn]
    [return: MaybeNull]
    public static T NeverHappen<T>()
    {
        // Keep these two as separate lines of code, so the debugger can come in during the assert dialog
        // that the exception's constructor displays, and the debugger can then be made to skip the throw
        // in order to continue the investigation.
        NotSupportedException exception = new("该行为不应发生，请联系开发者进一步确认");
        bool proceed = true; // allows debuggers to skip the throw statement
        if (proceed)
        {
            throw exception;
        }
        else
        {
#pragma warning disable CS8763 // 不应返回标记为 [DoesNotReturn] 的方法。
            return default;
#pragma warning restore CS8763 // 不应返回标记为 [DoesNotReturn] 的方法。
        }
    }

    /// <summary>
    /// 尝试抛出任务取消异常
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <param name="message">取消消息</param>
    /// <exception cref="TaskCanceledException">任务被取消</exception>
    [DebuggerStepThrough]
    [SuppressMessage("", "CA1068")]
    public static void TryThrowOnCanceled(CancellationToken token, string message)
    {
        if (token.IsCancellationRequested)
        {
            throw new TaskCanceledException("Image source has changed.");
        }
    }
}
