// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Binding;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.GachaLog.Factory;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 存档操作
/// </summary>
internal static class GachaArchives
{
    /// <summary>
    /// 初始化存档集合
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="collection">集合</param>
    public static void Initialize(AppDbContext appDbContext, out ObservableCollection<GachaArchive> collection)
    {
        try
        {
            collection = appDbContext.GachaArchives.AsNoTracking().ToObservableCollection();
        }
        catch (SqliteException ex)
        {
            string message = string.Format(SH.ServiceGachaLogArchiveCollectionUserdataCorruptedMessage, ex.Message);
            throw ThrowHelper.UserdataCorrupted(message, ex);
        }
    }
}