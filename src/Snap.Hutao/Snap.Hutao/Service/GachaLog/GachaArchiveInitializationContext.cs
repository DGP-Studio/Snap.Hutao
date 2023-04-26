// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model.Entity;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿存档初始化上下文
/// </summary>
internal readonly struct GachaArchiveInitializationContext
{
    /// <summary>
    /// 任务上下文
    /// </summary>
    public readonly ITaskContext TaskContext;

    /// <summary>
    /// Uid
    /// </summary>
    public readonly string Uid;

    /// <summary>
    /// 数据集
    /// </summary>
    public readonly DbSet<GachaArchive> GachaArchives;

    /// <summary>
    /// 存档集合
    /// </summary>
    public readonly ObservableCollection<GachaArchive> ArchiveCollection;

    /// <summary>
    /// 初始化一个新的祈愿存档初始化上下文
    /// </summary>
    /// <param name="taskContext">任务上下文</param>
    /// <param name="uid">uid</param>
    /// <param name="gachaArchives">数据集</param>
    /// <param name="archiveCollection">存档集合</param>
    public GachaArchiveInitializationContext(ITaskContext taskContext, string uid, DbSet<GachaArchive> gachaArchives, ObservableCollection<GachaArchive> archiveCollection)
    {
        TaskContext = taskContext;
        Uid = uid;
        GachaArchives = gachaArchives;
        ArchiveCollection = archiveCollection;
    }
}