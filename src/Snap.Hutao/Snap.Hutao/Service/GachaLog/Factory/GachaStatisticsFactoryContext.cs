// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Service.GachaLog.Factory;

internal readonly struct GachaStatisticsFactoryContext
{
    public readonly IServiceProvider ServiceProvider;
    public readonly ImmutableArray<Model.Entity.GachaItem> Items;
    public readonly ImmutableArray<HistoryWishBuilder> HistoryWishBuilders;
    public readonly GachaLogServiceMetadataContext Metadata;
    private readonly AppOptions appOptions;

    public GachaStatisticsFactoryContext(IServiceProvider serviceProvider, ImmutableArray<Model.Entity.GachaItem> items, ImmutableArray<HistoryWishBuilder> historyWishBuilders, GachaLogServiceMetadataContext metadata)
    {
        ServiceProvider = serviceProvider;
        Items = items;
        HistoryWishBuilders = historyWishBuilders;
        Metadata = metadata;
        appOptions = serviceProvider.GetRequiredService<AppOptions>();
    }

    public bool IsUnobtainedWishItemVisible { get => appOptions.IsUnobtainedWishItemVisible.Value; }

    public bool IsEmptyHistoryWishVisible { get => appOptions.IsEmptyHistoryWishVisible.Value; }
}