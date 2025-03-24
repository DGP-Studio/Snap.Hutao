// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.UI.Xaml.View.Page;
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

        this.DataContext<MainViewModel>()?.AttachXamlElement(BackgroundImagePresenter);

        INavigationService navigationService = serviceProvider.GetRequiredService<INavigationService>();
        navigationService.AttachXamlElement(NavView, ContentFrame);
        navigationService.Navigate<AnnouncementPage>(INavigationCompletionSource.Default, true);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        this.DataContext<MainViewModel>()?.Uninitialize();
    }
}