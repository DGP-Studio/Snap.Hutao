// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.ViewModel;
using Snap.Hutao.ViewModel.User;

namespace Snap.Hutao.UI.Xaml.View;

internal sealed partial class MainView : UserControl, IDataContextInitialized
{
    public MainView()
    {
        InitializeComponent();
        Unloaded += OnUnloaded;
    }

    public void OnDataContextInitialized(IServiceProvider serviceProvider)
    {
        UserView.InitializeDataContext<UserViewModel>(serviceProvider);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Unloaded -= OnUnloaded;
        UserView.DataContext<UserViewModel>()?.IsViewUnloaded.Value = true;
        this.DataContext<MainViewModel>()?.Uninitialize();
    }
}