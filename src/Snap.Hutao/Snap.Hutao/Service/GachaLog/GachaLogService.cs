// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录服务
/// </summary>
[Injection(InjectAs.Transient, typeof(IGachaLogService))]
internal class GachaLogService : IGachaLogService
{
    private readonly AppDbContext appDbContext;
    private readonly DbCurrent<GachaArchive, Message.GachaArchiveChangedMessage> dbCurrent;

    private ObservableCollection<GachaArchive>? archiveCollection;

    /// <summary>
    /// 构造一个新的祈愿记录服务
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="messenger">消息器</param>
    public GachaLogService(AppDbContext appDbContext, IMessenger messenger)
    {
        this.appDbContext = appDbContext;
        dbCurrent = new(appDbContext, appDbContext.GachaArchives, messenger);
    }

    /// <inheritdoc/>
    public GachaArchive? CurrentArchive
    {
        get => dbCurrent.Current;
        set => dbCurrent.Current = value;
    }

    /// <inheritdoc/>
    public ObservableCollection<GachaArchive> GetArchiveCollection()
    {
        return archiveCollection ??= new(appDbContext.GachaArchives.ToList());
    }
}