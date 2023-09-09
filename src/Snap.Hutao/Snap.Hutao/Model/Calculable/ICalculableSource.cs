// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Calculable;

/// <summary>
/// 可计算物品的源
/// </summary>
/// <typeparam name="TResult">可计算类型</typeparam>
[HighQuality]
internal interface ICalculableSource<TResult>
    where TResult : ICalculable
{
    /// <summary>
    /// 转换到可计算的对象
    /// </summary>
    /// <returns>可计算物品</returns>
    public TResult ToCalculable();
}

internal interface ITypedCalculableSource<TResult, T>
    where TResult : ICalculable
{
    /// <summary>
    /// 转换到可计算的对象
    /// </summary>
    /// <param name="param">索引</param>
    /// <returns>可计算物品</returns>
    public TResult ToCalculable(T param);
}