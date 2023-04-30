// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;

namespace Snap.Hutao.Service.Hutao;

/// <summary>
/// 胡桃用户服务
/// </summary>
internal interface IHutaoUserService : ICastableService
{
    /// <summary>
    /// 异步初始化
    /// </summary>
    /// <returns>任务</returns>
    ValueTask<bool> InitializeAsync();
}