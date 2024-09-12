// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hutao.GachaLog;

namespace Snap.Hutao.Service.GachaLog.Factory;

internal readonly struct GachaStatisticsFactoryContext
{
    public readonly ITaskContext TaskContext;
    public readonly HomaGachaLogClient GachaLogClient;
    public readonly List<Model.Entity.GachaItem> Items;
    public readonly List<HistoryWishBuilder> HistoryWishBuilders;
    public readonly GachaLogServiceMetadataContext Metadata;
    private readonly AppOptions appOptions;

    public GachaStatisticsFactoryContext(ITaskContext taskContext, HomaGachaLogClient gachaLogClient, List<Model.Entity.GachaItem> items, List<HistoryWishBuilder> historyWishBuilders, GachaLogServiceMetadataContext metadata, AppOptions appOptions)
    {
        TaskContext = taskContext;
        GachaLogClient = gachaLogClient;
        Items = items;
        HistoryWishBuilders = historyWishBuilders;
        Metadata = metadata;
        this.appOptions = appOptions;
    }

    public bool IsUnobtainedWishItemVisible { get => appOptions.IsUnobtainedWishItemVisible; }

    public bool IsEmptyHistoryWishVisible { get => appOptions.IsEmptyHistoryWishVisible; }
}