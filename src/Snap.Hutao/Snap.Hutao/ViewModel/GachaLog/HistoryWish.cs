// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.ViewModel.GachaLog;

internal sealed partial class HistoryWish : Wish, IAdvancedCollectionViewItem
{
    public string Version { get; set; } = default!;

    public Uri BannerImage { get; set; } = default!;

    public List<StatisticsItem> OrangeUpList { get; set; } = default!;

    public List<StatisticsItem> PurpleUpList { get; set; } = default!;

    public List<StatisticsItem> OrangeList { get; set; } = default!;

    public List<StatisticsItem> PurpleList { get; set; } = default!;

    public List<StatisticsItem> BlueList { get; set; } = default!;
}