// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Graphics;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.UI.Windowing.Abstraction;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window;

[Injection(InjectAs.Singleton)]
internal sealed partial class GuideWindow : Microsoft.UI.Xaml.Window,
    IXamlWindowExtendContentIntoTitleBar,
    IXamlWindowHasInitSize
{
    private const int MinWidth = 1000;
    private const int MinHeight = 650;

    public GuideWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsMaximizable = false;
            double scale = this.GetRasterizationScale();
            presenter.SetPreferredBounds(ScaledSizeInt32.CreateForWindow(1000, 650, this), ScaledSizeInt32.CreateForWindow(1200, 800, this));
        }

        this.InitializeController(serviceProvider);
    }

    public FrameworkElement TitleBarCaptionAccess { get => DraggableGrid; }

    public IEnumerable<FrameworkElement> TitleBarPassthrough { get => []; }

    public SizeInt32 InitSize { get => ScaledSizeInt32.CreateForWindow(MinWidth, MinHeight, this); }
}