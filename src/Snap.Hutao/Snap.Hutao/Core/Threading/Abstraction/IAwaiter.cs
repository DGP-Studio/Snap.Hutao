// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Threading.Abstraction;

/// <summary>
/// 用于给 await 确定异步返回的时机。
/// </summary>
internal interface IAwaiter : INotifyCompletion
{
    /// <summary>
    /// 获取一个状态，该状态表示正在异步等待的操作已经完成（成功完成或发生了异常）；此状态会被编译器自动调用。
    /// 在实现中，为了达到各种效果，可以灵活应用其值：可以始终为 true，或者始终为 false。
    /// </summary>
    bool IsCompleted { get; }

    /// <summary>
    /// 此方法会被编译器在 await 结束时自动调用以获取返回状态（包括异常）。
    /// </summary>
    void GetResult();
}

/// <summary>
/// 用于给 await 确定异步返回的时机，并获取到返回值。
/// </summary>
/// <typeparam name="TResult">异步返回的返回值类型。</typeparam>
internal interface IAwaiter<out TResult> : INotifyCompletion
{
    /// <summary>
    /// 获取一个状态，该状态表示正在异步等待的操作已经完成（成功完成或发生了异常）；此状态会被编译器自动调用。
    /// 在实现中，为了达到各种效果，可以灵活应用其值：可以始终为 true，或者始终为 false。
    /// </summary>
    bool IsCompleted { get; }

    /// <summary>
    /// 获取此异步等待操作的返回值，此方法会被编译器在 await 结束时自动调用以获取返回值（包括异常）。
    /// </summary>
    /// <returns>异步操作的返回值。</returns>
    TResult GetResult();
}