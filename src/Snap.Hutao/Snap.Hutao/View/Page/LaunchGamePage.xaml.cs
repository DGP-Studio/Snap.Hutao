// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.ViewModel.Game;
using System.Collections.ObjectModel;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 启动游戏页面
/// </summary>
[HighQuality]
internal sealed partial class LaunchGamePage : ScopedPage
{
    /// <summary>
    /// 构造一个新的启动游戏页面
    /// </summary>
    public LaunchGamePage()
    {
        InitializeWith<LaunchGameViewModel>();
        InitializeComponent();
    }

    public void OnAccountListViewReordered(object sender, DragItemsCompletedEventArgs e)
    {
        ListView listView = (ListView)sender;
        AdvancedCollectionView advancedCollectionView = (AdvancedCollectionView)listView.ItemsSource;
        ObservableCollection<GameAccount> gameAccounts = (ObservableCollection<GameAccount>)advancedCollectionView.Source;

        List<GameAccount> newGameAccounts = [];

        foreach (GameAccount gameAccount in gameAccounts)
        {
            newGameAccounts.Add(gameAccount.Clone());
        }

        ((LaunchGameViewModel)DataContext).ReorderGameAccounts(newGameAccounts);
    }
}
