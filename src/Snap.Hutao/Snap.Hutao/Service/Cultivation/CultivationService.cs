// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Service.Cultivation;

/// <summary>
/// 养成计算服务
/// </summary>
[Injection(InjectAs.Singleton, typeof(ICultivationService))]
internal class CultivationService : ICultivationService
{
    private readonly DbCurrent<CultivateProject, Message.CultivateProjectChangedMessage> dbCurrent;

    /// <summary>
    /// 构造一个新的养成计算服务
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="messenger">消息器</param>
    public CultivationService(AppDbContext appDbContext, IMessenger messenger)
    {
        dbCurrent = new(appDbContext.CultivateProjects, messenger);
    }

    /// <summary>
    /// 当前养成计划
    /// </summary>
    public CultivateProject? Current
    {
        get => dbCurrent.Current;
        set => dbCurrent.Current = value;
    }
}