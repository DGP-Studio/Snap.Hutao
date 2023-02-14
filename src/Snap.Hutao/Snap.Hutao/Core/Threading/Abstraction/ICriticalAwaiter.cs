// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Threading.Abstraction;

/// <summary>
/// 当执行关键代码（此代码中的错误可能给应用程序中的其他状态造成负面影响）时，
/// 用于给 await 确定异步返回的时机。
/// </summary>
internal interface ICriticalAwaiter : IAwaiter, ICriticalNotifyCompletion
{
}

/// <summary>
/// 当执行关键代码（此代码中的错误可能给应用程序中的其他状态造成负面影响）时，
/// 用于给 await 确定异步返回的时机，并获取到返回值。
/// </summary>
/// <typeparam name="TResult">异步返回的返回值类型。</typeparam>
internal interface ICriticalAwaiter<out TResult> : IAwaiter<TResult>, ICriticalNotifyCompletion
{
}