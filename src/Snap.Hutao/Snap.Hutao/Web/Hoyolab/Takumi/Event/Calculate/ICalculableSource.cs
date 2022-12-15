// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

/// <summary>
/// 可计算物品的源
/// </summary>
/// <typeparam name="T">可计算类型</typeparam>
public interface ICalculableSource<T>
    where T : ICalculable
{
    /// <summary>
    /// 转换到可计算的对象
    /// </summary>
    /// <returns>可计算物品</returns>
    public T ToCalculable();
}
