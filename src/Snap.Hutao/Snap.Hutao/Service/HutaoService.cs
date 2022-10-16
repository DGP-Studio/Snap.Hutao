// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hutao;

namespace Snap.Hutao.Service;

/// <summary>
/// 胡桃 API 服务
/// </summary>
[Injection(InjectAs.Transient)]
internal class HutaoService : IHutaoService
{
    private readonly HomaClient homaClient;

    /// <summary>
    /// 构造一个新的胡桃 API 服务
    /// </summary>
    /// <param name="homaClient">胡桃 API 客户端</param>
    /// <param name="memoryCache">内存缓存</param>
    public HutaoService(HomaClient homaClient, IMemoryCache memoryCache)
    {
        this.homaClient = homaClient;
    }
}