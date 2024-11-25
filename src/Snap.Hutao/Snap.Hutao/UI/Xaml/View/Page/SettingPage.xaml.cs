// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.ViewModel.Setting;

namespace Snap.Hutao.UI.Xaml.View.Page;

internal sealed partial class SettingPage : ScopedPage
{
    public SettingPage()
    {
        InitializeWith<SettingViewModel>();
        InitializeComponent();

        (DataContext as SettingViewModel)?.Initialize(new SettingScrollViewerAccess(RootScrollViewer, GachaLogBorder));
    }

    private class SettingScrollViewerAccess : ISettingScrollViewerAccessor
    {
        public SettingScrollViewerAccess(ScrollViewer scrollViewer, Border gachaLogBorder)
        {
            ScrollViewer = scrollViewer;
            GachaLogBorder = gachaLogBorder;
        }

        public ScrollViewer ScrollViewer { get; }

        public Border GachaLogBorder { get; }
    }
}