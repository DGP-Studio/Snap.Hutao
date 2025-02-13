// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window;

[Injection(InjectAs.Transient)]
internal sealed partial class MainWindow : Microsoft.UI.Xaml.Window,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowHasInitSize
{
    public MainWindow(IServiceProvider serviceProvider)
    {
        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.PreferredMinimumSize = new(1000, 600);
        }

        InitializeComponent();
        this.InitializeController(serviceProvider);
    }

    public FrameworkElement TitleBarCaptionAccess { get => TitleBarView.DragArea; }

    public IEnumerable<FrameworkElement> TitleBarPassthrough { get => TitleBarView.Passthrough; }

    public string PersistRectKey { get => SettingKeys.WindowRect; }

    public string PersistScaleKey { get => SettingKeys.WindowScale; }

    public SizeInt32 InitSize { get; } = new(1200, 741);
}