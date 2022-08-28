// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Sign;

/// <summary>
/// 签到服务
/// </summary>
[Obsolete("米游社官方限制了签到功能")]
public interface ISignService
{
    /// <summary>
    /// 异步全部签到
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>任务</returns>
    Task<SignResult> SignForAllAsync(CancellationToken token);
}
