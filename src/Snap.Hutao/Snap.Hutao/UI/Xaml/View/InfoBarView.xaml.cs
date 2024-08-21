// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Notification;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Snap.Hutao.UI.Xaml.View;

[DependencyProperty("InfoBars", typeof(ObservableCollection<InfoBarOptions>))]
internal sealed partial class InfoBarView : UserControl
{
    private readonly IInfoBarService infoBarService;

    public InfoBarView()
    {
        InitializeComponent();
        DataContext = this;

        IServiceProvider serviceProvider = Ioc.Default;
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        InfoBars = infoBarService.Collection;
        InfoBars.CollectionChanged += OnInfoBarsCollectionChanged;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        InfoBars.CollectionChanged -= OnInfoBarsCollectionChanged;
    }

    private void OnInfoBarsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        _ = HandleInfoBarsCollectionChangedAsync(args);

        [SuppressMessage("", "SH003")]
        async Task HandleInfoBarsCollectionChangedAsync(NotifyCollectionChangedEventArgs args)
        {
            if (InfoBars.Count > 0)
            {
                VisibilityRoot.Visibility = Visibility.Visible;
            }

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        InfoBarPanelTransitionHelper.Source = ShowButtonBorder;
                        InfoBarPanelTransitionHelper.Target = InfoBarItemsBorder;
                        await InfoBarPanelTransitionHelper.StartAsync().ConfigureAwait(true);
                        break;
                    }
            }

            if (InfoBars.Count is 0)
            {
                InfoBarPanelTransitionHelper.Source = InfoBarItemsBorder;
                InfoBarPanelTransitionHelper.Target = ShowButtonBorder;
                await InfoBarPanelTransitionHelper.StartAsync().ConfigureAwait(true);
                VisibilityRoot.Visibility = Visibility.Collapsed;
            }
        }
    }

    private void OnInfoBarClosed(InfoBar sender, InfoBarClosedEventArgs args)
    {
        InfoBars.Remove((InfoBarOptions)sender.DataContext);
    }

    private void OnClearAllButtonClick(object sender, RoutedEventArgs e)
    {
        _ = RemoveInfoBarsAsync();

        [SuppressMessage("", "SH003")]
        async Task RemoveInfoBarsAsync()
        {
            while (InfoBars.Count > 0)
            {
                InfoBars.RemoveLast();
                await Task.Delay(50).ConfigureAwait(true);
            }
        }
    }
}