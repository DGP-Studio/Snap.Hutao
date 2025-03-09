// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Service.Notification;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Snap.Hutao.UI.Xaml.View;

[DependencyProperty("InfoBars", typeof(ObservableCollection<InfoBarOptions>))]
internal sealed partial class InfoBarView : UserControl
{
    public InfoBarView()
    {
        InitializeComponent();
        DataContext = this;

        IServiceProvider serviceProvider = Ioc.Default;
        IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
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
                if (VisibilityRoot is not null)
                {
                    VisibilityRoot.Visibility = Visibility.Visible;
                }
            }

            if (args.Action is NotifyCollectionChangedAction.Add)
            {
                if (InfoBarPanelTransitionHelper is not null)
                {
                    InfoBarPanelTransitionHelper.Source = ShowButtonBorder;
                    InfoBarPanelTransitionHelper.Target = InfoBarItemsBorder;

                    if (ShowButtonBorder is not null && VisualTreeHelper.GetParent(ShowButtonBorder) is not null &&
                        InfoBarItemsBorder is not null && VisualTreeHelper.GetParent(InfoBarItemsBorder) is not null)
                    {
                        await InfoBarPanelTransitionHelper.StartAsync().ConfigureAwait(true);
                    }
                }

                // After adding, InfoBars.Count is not 0, so we skip the following code
                Debug.Assert(InfoBars.Count > 0);
                return;
            }

            if (InfoBars.Count is 0)
            {
                if (InfoBarPanelTransitionHelper is not null)
                {
                    InfoBarPanelTransitionHelper.Source = InfoBarItemsBorder;
                    InfoBarPanelTransitionHelper.Target = ShowButtonBorder;

                    if (InfoBarItemsBorder is not null && VisualTreeHelper.GetParent(InfoBarItemsBorder) is not null &&
                        ShowButtonBorder is not null && VisualTreeHelper.GetParent(ShowButtonBorder) is not null)
                    {
                        await InfoBarPanelTransitionHelper.StartAsync().ConfigureAwait(true);
                    }
                }

                if (VisibilityRoot is not null)
                {
                    VisibilityRoot.Visibility = Visibility.Collapsed;
                }
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